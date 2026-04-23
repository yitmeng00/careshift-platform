-- Extensions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ENUMS
DO $$ BEGIN
  CREATE TYPE staff_role AS ENUM (
    'Admin', 'DepartmentLead', 'ChargeNurse', 'Doctor', 'Nurse', 'Receptionist'
  );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE shift_type AS ENUM ('Morning', 'Afternoon', 'Night');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE leave_status AS ENUM ('Pending', 'Approved', 'Rejected');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE leave_type AS ENUM ('Annual', 'Sick', 'Training', 'Personal');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE employment_type AS ENUM ('FullTime', 'PartTime', 'Contract');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE swap_status AS ENUM (
    'PendingRequestee', 'PendingAdmin', 'Approved', 'Rejected', 'Cancelled'
  );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

-- TABLES
CREATE TABLE IF NOT EXISTS departments (
  id          SERIAL          PRIMARY KEY,
  name        VARCHAR(100)    NOT NULL UNIQUE,
  description VARCHAR(500)
);

CREATE TABLE IF NOT EXISTS staff (
  id              SERIAL          PRIMARY KEY,
  full_name       VARCHAR(100)    NOT NULL,
  email           VARCHAR(200)    NOT NULL UNIQUE,
  password_hash   TEXT            NOT NULL,
  role            staff_role      NOT NULL,
  department_id   INT             NOT NULL REFERENCES departments(id) ON DELETE RESTRICT,
  is_active       BOOLEAN         NOT NULL DEFAULT TRUE,
  phone           VARCHAR(30),
  initials        VARCHAR(5)      NOT NULL,
  employment_type employment_type NOT NULL DEFAULT 'FullTime',
  created_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_staff_email           ON staff(email);
CREATE INDEX IF NOT EXISTS idx_staff_department_id   ON staff(department_id);
CREATE INDEX IF NOT EXISTS idx_staff_role            ON staff(role);
CREATE INDEX IF NOT EXISTS idx_staff_is_active       ON staff(is_active);

-- Each shift record represents one staff member's assigned block on a calendar day.
-- ShiftType drives start/end times (Morning 07-15, Afternoon 15-23, Night 23-07).
CREATE TABLE IF NOT EXISTS shifts (
  id            SERIAL          PRIMARY KEY,
  staff_id      INT             NOT NULL REFERENCES staff(id)       ON DELETE CASCADE,
  department_id INT             NOT NULL REFERENCES departments(id) ON DELETE RESTRICT,
  start_time    TIMESTAMPTZ     NOT NULL,
  end_time      TIMESTAMPTZ     NOT NULL,
  shift_type    shift_type      NOT NULL,
  notes         VARCHAR(500),
  created_at    TIMESTAMPTZ     NOT NULL DEFAULT NOW(),

  CONSTRAINT chk_shifts_end_after_start CHECK (end_time > start_time)
);

CREATE INDEX IF NOT EXISTS idx_shifts_staff_id       ON shifts(staff_id);
CREATE INDEX IF NOT EXISTS idx_shifts_department_id  ON shifts(department_id);
CREATE INDEX IF NOT EXISTS idx_shifts_start_time     ON shifts(start_time);

CREATE TABLE IF NOT EXISTS leave_requests (
  id              SERIAL        PRIMARY KEY,
  staff_id        INT           NOT NULL REFERENCES staff(id) ON DELETE CASCADE,
  leave_type      leave_type    NOT NULL,
  start_date      DATE          NOT NULL,
  end_date        DATE          NOT NULL,
  reason          VARCHAR(500)  NOT NULL,
  status          leave_status  NOT NULL DEFAULT 'Pending',
  reviewed_by_id  INT           REFERENCES staff(id) ON DELETE SET NULL,
  reviewed_at     TIMESTAMPTZ,
  review_note     VARCHAR(500),
  submitted_at    TIMESTAMPTZ   NOT NULL DEFAULT NOW(),

  CONSTRAINT chk_leave_end_gte_start CHECK (end_date >= start_date)
);

CREATE INDEX IF NOT EXISTS idx_leave_staff_id    ON leave_requests(staff_id);
CREATE INDEX IF NOT EXISTS idx_leave_status      ON leave_requests(status);

-- Leave audit trail (immutable event log per leave request)
CREATE TABLE IF NOT EXISTS leave_audit_entries (
  id               SERIAL        PRIMARY KEY,
  leave_request_id INT           NOT NULL REFERENCES leave_requests(id) ON DELETE CASCADE,
  at               TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
  by               VARCHAR(100)  NOT NULL,
  action           VARCHAR(100)  NOT NULL,  -- 'submitted' | 'approved' | 'rejected'
  note             VARCHAR(500)
);

CREATE INDEX IF NOT EXISTS idx_leave_audit_leave_id ON leave_audit_entries(leave_request_id);

-- Shift swap requests
-- 3-step workflow: PendingRequestee → PendingAdmin → Approved/Rejected
CREATE TABLE IF NOT EXISTS shift_swap_requests (
  id                  SERIAL      PRIMARY KEY,
  requester_id        INT         NOT NULL REFERENCES staff(id) ON DELETE RESTRICT,
  requestee_id        INT         NOT NULL REFERENCES staff(id) ON DELETE RESTRICT,
  requester_shift_id  INT         NOT NULL REFERENCES shifts(id) ON DELETE RESTRICT,
  requestee_shift_id  INT         NOT NULL REFERENCES shifts(id) ON DELETE RESTRICT,
  reason              VARCHAR(500) NOT NULL,
  status              swap_status NOT NULL DEFAULT 'PendingRequestee',
  submitted_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  CONSTRAINT chk_swap_different_staff CHECK (requester_id <> requestee_id),
  CONSTRAINT chk_swap_different_shifts CHECK (requester_shift_id <> requestee_shift_id)
);

CREATE INDEX IF NOT EXISTS idx_swap_requester_id ON shift_swap_requests(requester_id);
CREATE INDEX IF NOT EXISTS idx_swap_requestee_id ON shift_swap_requests(requestee_id);
CREATE INDEX IF NOT EXISTS idx_swap_status       ON shift_swap_requests(status);

-- Swap audit trail
CREATE TABLE IF NOT EXISTS swap_audit_entries (
  id                    SERIAL       PRIMARY KEY,
  shift_swap_request_id INT          NOT NULL REFERENCES shift_swap_requests(id) ON DELETE CASCADE,
  at                    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
  by                    VARCHAR(100) NOT NULL,
  action                VARCHAR(100) NOT NULL,  -- 'submitted' | 'accepted' | 'declined' | 'approved' | 'rejected'
  note                  VARCHAR(500)
);

CREATE INDEX IF NOT EXISTS idx_swap_audit_swap_id ON swap_audit_entries(shift_swap_request_id);

-- Global audit log (system-wide action history)
CREATE TABLE IF NOT EXISTS audit_logs (
  id           SERIAL       PRIMARY KEY,
  staff_id     INT          REFERENCES staff(id) ON DELETE SET NULL,
  action       VARCHAR(200) NOT NULL,
  entity_type  VARCHAR(100) NOT NULL,
  entity_id    INT,
  detail       VARCHAR(500),
  icon         VARCHAR(10),
  performed_by VARCHAR(100) NOT NULL,
  timestamp    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_audit_log_staff_id  ON audit_logs(staff_id);
CREATE INDEX IF NOT EXISTS idx_audit_log_timestamp ON audit_logs(timestamp DESC);
CREATE INDEX IF NOT EXISTS idx_audit_log_entity    ON audit_logs(entity_type, entity_id);

-- VIEWS (optional conveniences for reporting)
CREATE OR REPLACE VIEW v_weekly_hours AS
SELECT
  s.id              AS staff_id,
  s.full_name,
  s.role,
  d.name            AS department,
  DATE_TRUNC('week', sh.start_time) AS week_start,
  COUNT(*)          AS shift_count,
  SUM(
    EXTRACT(EPOCH FROM (sh.end_time - sh.start_time)) / 3600
  )::NUMERIC(5,1)   AS total_hours
FROM shifts sh
JOIN staff       s ON sh.staff_id      = s.id
JOIN departments d ON sh.department_id = d.id
WHERE s.is_active = TRUE
GROUP BY s.id, s.full_name, s.role, d.name, DATE_TRUNC('week', sh.start_time);

COMMENT ON VIEW v_weekly_hours IS
  'Aggregate shift hours per staff member per calendar week — used by the overtime tracker.';
