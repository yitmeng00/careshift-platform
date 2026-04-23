-- DEPARTMENTS
INSERT INTO departments (name, description) VALUES
  ('Emergency',  'Emergency department'),
  ('Cardiology', 'Cardiology department'),
  ('Pediatrics', 'Pediatrics department'),
  ('ICU',        'Intensive Care Unit'),
  ('Front Desk', 'Front desk and reception'),
  ('Radiology',  'Radiology department'),
  ('Oncology',   'Oncology department')
ON CONFLICT (name) DO NOTHING;

-- =============================================================================
-- STAFF
-- Passwords:
--   admin@hospital.org  → admin123
--   all others          → password123
--
-- BCrypt hashes shown are illustrative; the application seeds real hashes.
-- Replace with output of:
--   SELECT crypt('password123', gen_salt('bf', 10));
-- =============================================================================

INSERT INTO staff
  (full_name,         email,                  password_hash,  role,            department_id,     initials, employment_type, phone,      is_active)
VALUES
  ('Dr. Sarah Chen',  's.chen@hospital.org',  '$HASH$',  'Doctor',          (SELECT id FROM departments WHERE name='Emergency'), 'SC', 'FullTime',  '555-0101', TRUE),
  ('Dr. James Park',  'j.park@hospital.org',  '$HASH$',  'Doctor',          (SELECT id FROM departments WHERE name='Cardiology'), 'JP', 'FullTime',  '555-0102', TRUE),
  ('Dr. Priya Nair',  'p.nair@hospital.org',  '$HASH$',  'Doctor',          (SELECT id FROM departments WHERE name='Pediatrics'), 'PN', 'PartTime',  '555-0103', TRUE),
  ('Emma White',      'e.white@hospital.org', '$HASH$',  'Nurse',           (SELECT id FROM departments WHERE name='Emergency'), 'EW', 'FullTime',  '555-0104', TRUE),
  ('Luis Torres',     'l.torres@hospital.org','$HASH$',  'Nurse',           (SELECT id FROM departments WHERE name='ICU'), 'LT', 'FullTime',  '555-0105', TRUE),
  ('Aisha Johnson',   'a.johnson@hospital.org','$HASH$', 'Nurse',           (SELECT id FROM departments WHERE name='Pediatrics'), 'AJ', 'FullTime',  '555-0106', TRUE),
  ('Kira Patel',      'k.patel@hospital.org', '$HASH$',  'Nurse',           (SELECT id FROM departments WHERE name='Cardiology'), 'KP', 'PartTime',  '555-0107', TRUE),
  ('Mark Stevens',    'm.stevens@hospital.org','$HASH$', 'Receptionist',    (SELECT id FROM departments WHERE name='Front Desk'), 'MS', 'FullTime',  '555-0108', TRUE),
  ('Lisa Wong',       'l.wong@hospital.org',  '$HASH$',  'Receptionist',    (SELECT id FROM departments WHERE name='Front Desk'), 'LW', 'PartTime',  '555-0109', TRUE),
  ('Dr. Marcus Kim',  'm.kim@hospital.org',   '$HASH$',  'DepartmentLead',  (SELECT id FROM departments WHERE name='Emergency'), 'MK', 'FullTime',  '555-0110', TRUE),
  ('Diane Foster',    'd.foster@hospital.org','$HASH$',  'ChargeNurse',     (SELECT id FROM departments WHERE name='ICU'), 'DF', 'FullTime',  '555-0111', TRUE),
  ('Admin User',      'admin@hospital.org',   '$ADMIN_HASH$', 'Admin',      (SELECT id FROM departments WHERE name='Emergency'), 'AD', 'FullTime',  '555-0100', TRUE)
ON CONFLICT (email) DO NOTHING;

-- NOTE: Replace $HASH$ and $ADMIN_HASH$ with real BCrypt values before running manually.
-- The application's DbSeeder generates correct hashes automatically on first run.

-- =============================================================================
-- SHIFTS (relative to today — adjust dates as needed)
-- Using NOW() as a base; shift at day offset D, hour H.
-- Shift durations: Morning 07-15, Afternoon 15-23, Night 23-07 (next day)
-- =============================================================================

-- Helper: current Monday of this week
WITH week_start AS (
  SELECT DATE_TRUNC('week', NOW()) AS mon
),
staff_ids AS (
  SELECT id, email FROM staff
)
INSERT INTO shifts (staff_id, department_id, start_time, end_time, shift_type)
SELECT s.id,
       (SELECT id FROM departments WHERE name = dept),
       mon + INTERVAL '1 day' * day_off + start_h,
       mon + INTERVAL '1 day' * end_day_off + end_h,
       stype::shift_type
FROM (VALUES
  -- Dr. Sarah Chen — Emergency
  ('s.chen@hospital.org', 'Emergency', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
  ('s.chen@hospital.org', 'Emergency', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
  ('s.chen@hospital.org', 'Emergency', 2, '15:00:00'::interval, 2, '23:00:00'::interval, 'Afternoon'),
  ('s.chen@hospital.org', 'Emergency', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
  ('s.chen@hospital.org', 'Emergency', 5, '07:00:00'::interval, 5, '15:00:00'::interval, 'Morning'),
  -- Dr. James Park — Cardiology
  ('j.park@hospital.org', 'Cardiology', 0, '15:00:00'::interval, 0, '23:00:00'::interval, 'Afternoon'),
  ('j.park@hospital.org', 'Cardiology', 1, '15:00:00'::interval, 1, '23:00:00'::interval, 'Afternoon'),
  ('j.park@hospital.org', 'Cardiology', 3, '07:00:00'::interval, 3, '15:00:00'::interval, 'Morning'),
  ('j.park@hospital.org', 'Cardiology', 4, '23:00:00'::interval, 5, '07:00:00'::interval, 'Night'),
  ('j.park@hospital.org', 'Cardiology', 6, '07:00:00'::interval, 6, '15:00:00'::interval, 'Morning'),
  -- Emma White — Emergency nurse (5 morning shifts)
  ('e.white@hospital.org', 'Emergency', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
  ('e.white@hospital.org', 'Emergency', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
  ('e.white@hospital.org', 'Emergency', 2, '07:00:00'::interval, 2, '15:00:00'::interval, 'Morning'),
  ('e.white@hospital.org', 'Emergency', 3, '07:00:00'::interval, 3, '15:00:00'::interval, 'Morning'),
  ('e.white@hospital.org', 'Emergency', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
  -- Luis Torres — ICU night shifts
  ('l.torres@hospital.org', 'ICU', 0, '23:00:00'::interval, 1, '07:00:00'::interval, 'Night'),
  ('l.torres@hospital.org', 'ICU', 1, '23:00:00'::interval, 2, '07:00:00'::interval, 'Night'),
  ('l.torres@hospital.org', 'ICU', 2, '23:00:00'::interval, 3, '07:00:00'::interval, 'Night'),
  ('l.torres@hospital.org', 'ICU', 4, '15:00:00'::interval, 4, '23:00:00'::interval, 'Afternoon'),
  ('l.torres@hospital.org', 'ICU', 5, '15:00:00'::interval, 5, '23:00:00'::interval, 'Afternoon'),
  ('l.torres@hospital.org', 'ICU', 6, '15:00:00'::interval, 6, '23:00:00'::interval, 'Afternoon'),
  -- Aisha Johnson — Pediatrics
  ('a.johnson@hospital.org', 'Pediatrics', 0, '15:00:00'::interval, 0, '23:00:00'::interval, 'Afternoon'),
  ('a.johnson@hospital.org', 'Pediatrics', 2, '07:00:00'::interval, 2, '15:00:00'::interval, 'Morning'),
  ('a.johnson@hospital.org', 'Pediatrics', 3, '07:00:00'::interval, 3, '15:00:00'::interval, 'Morning'),
  ('a.johnson@hospital.org', 'Pediatrics', 5, '15:00:00'::interval, 5, '23:00:00'::interval, 'Afternoon'),
  ('a.johnson@hospital.org', 'Pediatrics', 6, '07:00:00'::interval, 6, '15:00:00'::interval, 'Morning'),
  -- Mark Stevens — Front Desk
  ('m.stevens@hospital.org', 'Front Desk', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
  ('m.stevens@hospital.org', 'Front Desk', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
  ('m.stevens@hospital.org', 'Front Desk', 2, '07:00:00'::interval, 2, '15:00:00'::interval, 'Morning'),
  ('m.stevens@hospital.org', 'Front Desk', 3, '07:00:00'::interval, 3, '15:00:00'::interval, 'Morning'),
  ('m.stevens@hospital.org', 'Front Desk', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
  -- Dr. Marcus Kim — Emergency (Dept Lead)
  ('m.kim@hospital.org', 'Emergency', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
  ('m.kim@hospital.org', 'Emergency', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
  ('m.kim@hospital.org', 'Emergency', 3, '15:00:00'::interval, 3, '23:00:00'::interval, 'Afternoon'),
  ('m.kim@hospital.org', 'Emergency', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
  ('m.kim@hospital.org', 'Emergency', 5, '07:00:00'::interval, 5, '15:00:00'::interval, 'Morning'),
  -- Diane Foster — ICU (Charge Nurse)
  ('d.foster@hospital.org', 'ICU', 0, '23:00:00'::interval, 1, '07:00:00'::interval, 'Night'),
  ('d.foster@hospital.org', 'ICU', 1, '23:00:00'::interval, 2, '07:00:00'::interval, 'Night'),
  ('d.foster@hospital.org', 'ICU', 2, '23:00:00'::interval, 3, '07:00:00'::interval, 'Night'),
  ('d.foster@hospital.org', 'ICU', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
  ('d.foster@hospital.org', 'ICU', 5, '07:00:00'::interval, 5, '15:00:00'::interval, 'Morning')
) AS t(email, dept, day_off, start_h, end_day_off, end_h, stype)
JOIN staff_ids si ON si.email = t.email
CROSS JOIN week_start;

-- LEAVE REQUESTS
WITH staff_ids AS (SELECT id, email FROM staff)
INSERT INTO leave_requests
  (staff_id, leave_type, start_date, end_date, reason, status, reviewed_by_id, reviewed_at, review_note, submitted_at)
VALUES
  (
    (SELECT id FROM staff WHERE email='j.park@hospital.org'),
    'Annual', CURRENT_DATE + 3, CURRENT_DATE + 4,
    'Family event', 'Pending', NULL, NULL, NULL,
    NOW() - INTERVAL '2 days'
  ),
  (
    (SELECT id FROM staff WHERE email='e.white@hospital.org'),
    'Annual', CURRENT_DATE + 8, CURRENT_DATE + 10,
    'Vacation', 'Pending', NULL, NULL, NULL,
    NOW() - INTERVAL '3 days'
  ),
  (
    (SELECT id FROM staff WHERE email='a.johnson@hospital.org'),
    'Sick', CURRENT_DATE + 1, CURRENT_DATE + 1,
    'Flu symptoms', 'Approved',
    (SELECT id FROM staff WHERE email='admin@hospital.org'),
    NOW() - INTERVAL '1 hour', 'Cover arranged with Kira',
    NOW() - INTERVAL '1 day'
  ),
  (
    (SELECT id FROM staff WHERE email='s.chen@hospital.org'),
    'Training', CURRENT_DATE + 2, CURRENT_DATE + 2,
    'ACLS refresher', 'Rejected',
    (SELECT id FROM staff WHERE email='admin@hospital.org'),
    NOW() - INTERVAL '5 days', 'Minimum staffing not met',
    NOW() - INTERVAL '6 days'
  ),
  (
    (SELECT id FROM staff WHERE email='l.torres@hospital.org'),
    'Personal', CURRENT_DATE + 7, CURRENT_DATE + 7,
    'Personal appointment', 'Pending', NULL, NULL, NULL,
    NOW() - INTERVAL '1 day'
  );

-- LEAVE AUDIT ENTRIES
INSERT INTO leave_audit_entries (leave_request_id, at, by, action, note)
SELECT lr.id, lr.submitted_at, s.full_name, 'submitted', NULL
FROM leave_requests lr
JOIN staff s ON s.id = lr.staff_id;

INSERT INTO leave_audit_entries (leave_request_id, at, by, action, note)
SELECT lr.id, lr.reviewed_at, rev.full_name, lr.status::text, lr.review_note
FROM leave_requests lr
JOIN staff rev ON rev.id = lr.reviewed_by_id
WHERE lr.reviewed_by_id IS NOT NULL;

-- SHIFT SWAP REQUESTS
-- Swap 1: Emma White wants to swap her Mon morning shift with Aisha Johnson
-- Status: PendingRequestee (Aisha hasn't responded yet)
INSERT INTO shift_swap_requests
  (requester_id, requestee_id, requester_shift_id, requestee_shift_id, reason, status, submitted_at)
SELECT
  (SELECT id FROM staff WHERE email='e.white@hospital.org'),
  (SELECT id FROM staff WHERE email='a.johnson@hospital.org'),
  (SELECT sh.id FROM shifts sh
   JOIN staff s ON s.id = sh.staff_id
   WHERE s.email = 'e.white@hospital.org' AND sh.shift_type = 'Morning'
   ORDER BY sh.start_time LIMIT 1),
  (SELECT sh.id FROM shifts sh
   JOIN staff s ON s.id = sh.staff_id
   WHERE s.email = 'a.johnson@hospital.org' AND sh.shift_type = 'Afternoon'
   ORDER BY sh.start_time LIMIT 1),
  'Childcare conflict on Monday morning',
  'PendingRequestee',
  NOW() - INTERVAL '1 day';

-- Swap 2: Luis Torres ↔ Kira Patel — Kira already accepted, pending admin
INSERT INTO shift_swap_requests
  (requester_id, requestee_id, requester_shift_id, requestee_shift_id, reason, status, submitted_at)
SELECT
  (SELECT id FROM staff WHERE email='l.torres@hospital.org'),
  (SELECT id FROM staff WHERE email='k.patel@hospital.org'),
  (SELECT sh.id FROM shifts sh
   JOIN staff s ON s.id = sh.staff_id
   WHERE s.email = 'l.torres@hospital.org' AND sh.shift_type = 'Night'
   ORDER BY sh.start_time LIMIT 1),
  (SELECT sh.id FROM shifts sh
   JOIN staff s ON s.id = sh.staff_id
   WHERE s.email = 'k.patel@hospital.org'
   ORDER BY sh.start_time LIMIT 1),
  'Medical appointment on Friday afternoon',
  'PendingAdmin',
  NOW() - INTERVAL '2 days';

-- SWAP AUDIT ENTRIES
INSERT INTO swap_audit_entries (shift_swap_request_id, at, by, action, note)
SELECT id, submitted_at, 'Emma White', 'submitted', NULL
FROM shift_swap_requests WHERE reason LIKE 'Childcare%';

INSERT INTO swap_audit_entries (shift_swap_request_id, at, by, action, note)
SELECT id, submitted_at, 'Luis Torres', 'submitted', NULL
FROM shift_swap_requests WHERE reason LIKE 'Medical%';

INSERT INTO swap_audit_entries (shift_swap_request_id, at, by, action, note)
SELECT id, submitted_at + INTERVAL '2 hours', 'Kira Patel', 'accepted', 'Happy to swap'
FROM shift_swap_requests WHERE reason LIKE 'Medical%';

-- GLOBAL AUDIT LOG
INSERT INTO audit_logs (staff_id, action, entity_type, entity_id, detail, icon, performed_by, timestamp)
VALUES
  ((SELECT id FROM staff WHERE email='admin@hospital.org'),
   'Approved leave', 'LeaveRequest', NULL,
   'Aisha Johnson · Sick Leave', '✓', 'Admin User', NOW() - INTERVAL '1 hour'),

  ((SELECT id FROM staff WHERE email='admin@hospital.org'),
   'Shift reassigned', 'Shift', NULL,
   'Dr. James Park · Night → Afternoon', '↔', 'Admin User', NOW() - INTERVAL '12 hours'),

  ((SELECT id FROM staff WHERE email='l.torres@hospital.org'),
   'Leave submitted', 'LeaveRequest', NULL,
   'Personal leave', '+', 'Luis Torres', NOW() - INTERVAL '1 day'),

  ((SELECT id FROM staff WHERE email='e.white@hospital.org'),
   'Swap requested', 'ShiftSwapRequest', NULL,
   'Mon Morning with Aisha Johnson', '↔', 'Emma White', NOW() - INTERVAL '1 day'),

  ((SELECT id FROM staff WHERE email='k.patel@hospital.org'),
   'Swap accepted', 'ShiftSwapRequest', NULL,
   'Luis Torres · Night shift', '✓', 'Kira Patel', NOW() - INTERVAL '2 days'),

  ((SELECT id FROM staff WHERE email='j.park@hospital.org'),
   'Leave submitted', 'LeaveRequest', NULL,
   'Annual Leave · 3 days', '+', 'Dr. James Park', NOW() - INTERVAL '2 days'),

  ((SELECT id FROM staff WHERE email='admin@hospital.org'),
   'Rejected leave', 'LeaveRequest', NULL,
   'Dr. Sarah Chen · Training', '×', 'Admin User', NOW() - INTERVAL '5 days'),

  (NULL,
   'System seeded', 'System', NULL,
   'Initial seed data loaded', '+', 'System', NOW());
