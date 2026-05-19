-- =============================================================================
-- CareShift — Seed data
-- Run AFTER schema.sql on a fresh database.
--
-- Credentials:
--   admin@hospital.org      password: admin123
--   all others              password: password123
--
-- Hashes are BCrypt work-factor 10, compatible with BCrypt.Net-Next.
-- =============================================================================

-- =============================================================================
-- DEPARTMENTS
-- =============================================================================
INSERT INTO "Departments" ("Name", "Description") VALUES
  ('Emergency',  'Emergency department'),
  ('Cardiology', 'Cardiology department'),
  ('Pediatrics', 'Pediatrics department'),
  ('ICU',        'Intensive Care Unit'),
  ('Front Desk', 'Front desk and reception')
ON CONFLICT ("Name") DO NOTHING;

-- =============================================================================
-- STAFF
-- BCrypt hashes (work factor 10):
--   password123 → $2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.
--   admin123    → $2a$10$53f9rbh3ZPUNEnM1FmH8Fu7LJA2I0sWqHzSvlPwESIoCtxTtb9tEK
-- =============================================================================
INSERT INTO "Staff" ("FullName", "Email", "PasswordHash", "Role", "DepartmentId", "Initials", "EmploymentType", "Phone", "IsActive", "CreatedAt")
SELECT
    s."FullName",
    s."Email",
    s."PasswordHash",
    s."Role",
    d."Id",
    s."Initials",
    s."EmploymentType",
    s."Phone",
    TRUE,
    NOW()
FROM (VALUES
    ('Dr. Sarah Chen',  's.chen@hospital.org',    '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Doctor',         'Emergency',  'SC', 'FullTime',  '555-0101'),
    ('Dr. James Park',  'j.park@hospital.org',    '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Doctor',         'Cardiology', 'JP', 'FullTime',  '555-0102'),
    ('Dr. Priya Nair',  'p.nair@hospital.org',    '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Doctor',         'Pediatrics', 'PN', 'PartTime',  '555-0103'),
    ('Emma White',      'e.white@hospital.org',   '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Nurse',          'Emergency',  'EW', 'FullTime',  '555-0104'),
    ('Luis Torres',     'l.torres@hospital.org',  '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Nurse',          'ICU',        'LT', 'FullTime',  '555-0105'),
    ('Aisha Johnson',   'a.johnson@hospital.org', '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Nurse',          'Pediatrics', 'AJ', 'FullTime',  '555-0106'),
    ('Kira Patel',      'k.patel@hospital.org',   '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Nurse',          'Cardiology', 'KP', 'PartTime',  '555-0107'),
    ('Mark Stevens',    'm.stevens@hospital.org', '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Receptionist',   'Front Desk', 'MS', 'FullTime',  '555-0108'),
    ('Lisa Wong',       'l.wong@hospital.org',    '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'Receptionist',   'Front Desk', 'LW', 'PartTime',  '555-0109'),
    ('Dr. Marcus Kim',  'm.kim@hospital.org',     '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'DepartmentLead', 'Emergency',  'MK', 'FullTime',  '555-0110'),
    ('Diane Foster',    'd.foster@hospital.org',  '$2a$10$AwI709DrJ34mHHFsZZ64.egKP8R2yI7zloan51vjcQlv1wFEscf3.', 'ChargeNurse',    'ICU',        'DF', 'FullTime',  '555-0111'),
    ('Admin User',      'admin@hospital.org',     '$2a$10$53f9rbh3ZPUNEnM1FmH8Fu7LJA2I0sWqHzSvlPwESIoCtxTtb9tEK', 'Admin',         'Emergency',  'AU', 'FullTime',  '555-0100')
) AS s("FullName", "Email", "PasswordHash", "Role", "DeptName", "Initials", "EmploymentType", "Phone")
JOIN "Departments" d ON d."Name" = s."DeptName"
ON CONFLICT ("Email") DO NOTHING;

-- =============================================================================
-- SHIFTS  (current week, Monday-anchored, times in UTC)
-- Morning: 07:00-15:00 | Afternoon: 15:00-23:00 | Night: 23:00-07:00(+1d)
--
-- DATE_TRUNC('week', NOW() AT TIME ZONE 'UTC') gives Monday 00:00 UTC.
-- day_off 0=Mon … 6=Sun
-- =============================================================================
WITH
  mon AS (
    -- Monday 00:00 UTC as timestamptz
    SELECT (DATE_TRUNC('week', NOW() AT TIME ZONE 'UTC')) AT TIME ZONE 'UTC' AS d
  )
INSERT INTO "Shifts" ("StaffId", "DepartmentId", "StartTime", "EndTime", "ShiftType", "CreatedAt")
SELECT
    stf."Id",
    dept."Id",
    mon.d + t.day_off     * INTERVAL '1 day' + t.start_h,
    mon.d + t.end_day_off * INTERVAL '1 day' + t.end_h,
    t.shift_type,
    NOW()
FROM (VALUES
    -- Dr. Sarah Chen — Emergency (Mon/Tue Morning, Wed Afternoon, Fri/Sat Morning)
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
    -- Emma White — Emergency (5× Morning Mon–Fri)
    ('e.white@hospital.org', 'Emergency', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
    ('e.white@hospital.org', 'Emergency', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
    ('e.white@hospital.org', 'Emergency', 2, '07:00:00'::interval, 2, '15:00:00'::interval, 'Morning'),
    ('e.white@hospital.org', 'Emergency', 3, '07:00:00'::interval, 3, '15:00:00'::interval, 'Morning'),
    ('e.white@hospital.org', 'Emergency', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
    -- Luis Torres — ICU (Mon-Wed Night, Fri-Sun Afternoon)
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
    -- Mark Stevens — Front Desk (Mon–Fri Morning)
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
    -- Diane Foster — ICU (Charge Nurse, Mon-Wed Night, Fri-Sat Morning)
    ('d.foster@hospital.org', 'ICU', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
    ('d.foster@hospital.org', 'ICU', 1, '07:00:00'::interval, 1, '15:00:00'::interval, 'Morning'),
    ('d.foster@hospital.org', 'ICU', 2, '23:00:00'::interval, 3, '07:00:00'::interval, 'Night'),
    ('d.foster@hospital.org', 'ICU', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning'),
    ('d.foster@hospital.org', 'ICU', 5, '07:00:00'::interval, 5, '15:00:00'::interval, 'Morning'),
    -- Kira Patel — Cardiology (Tue/Thu Afternoon)
    ('k.patel@hospital.org', 'Cardiology', 1, '15:00:00'::interval, 1, '23:00:00'::interval, 'Afternoon'),
    ('k.patel@hospital.org', 'Cardiology', 3, '15:00:00'::interval, 3, '23:00:00'::interval, 'Afternoon'),
    -- Dr. Priya Nair — Pediatrics (Mon/Wed/Fri Morning)
    ('p.nair@hospital.org', 'Pediatrics', 0, '07:00:00'::interval, 0, '15:00:00'::interval, 'Morning'),
    ('p.nair@hospital.org', 'Pediatrics', 2, '07:00:00'::interval, 2, '15:00:00'::interval, 'Morning'),
    ('p.nair@hospital.org', 'Pediatrics', 4, '07:00:00'::interval, 4, '15:00:00'::interval, 'Morning')
) AS t(email, dept_name, day_off, start_h, end_day_off, end_h, shift_type)
JOIN "Staff"       stf  ON stf."Email" = t.email
JOIN "Departments" dept ON dept."Name" = t.dept_name
CROSS JOIN mon;

-- =============================================================================
-- LEAVE REQUESTS
-- =============================================================================
INSERT INTO "LeaveRequests"
    ("StaffId", "LeaveType", "StartDate", "EndDate", "Reason", "Status",
     "ReviewedById", "ReviewedAt", "ReviewNote", "SubmittedAt")
VALUES
    (
        (SELECT "Id" FROM "Staff" WHERE "Email"='j.park@hospital.org'),
        'Annual', CURRENT_DATE + 3, CURRENT_DATE + 4,
        'Family event', 'Pending', NULL, NULL, NULL,
        NOW() - INTERVAL '2 days'
    ),
    (
        (SELECT "Id" FROM "Staff" WHERE "Email"='e.white@hospital.org'),
        'Annual', CURRENT_DATE + 8, CURRENT_DATE + 10,
        'Vacation', 'Pending', NULL, NULL, NULL,
        NOW() - INTERVAL '3 days'
    ),
    (
        (SELECT "Id" FROM "Staff" WHERE "Email"='a.johnson@hospital.org'),
        'Sick', CURRENT_DATE + 1, CURRENT_DATE + 1,
        'Flu symptoms', 'Approved',
        (SELECT "Id" FROM "Staff" WHERE "Email"='admin@hospital.org'),
        NOW() - INTERVAL '1 hour', 'Cover arranged with Kira',
        NOW() - INTERVAL '1 day'
    ),
    (
        (SELECT "Id" FROM "Staff" WHERE "Email"='s.chen@hospital.org'),
        'Annual', CURRENT_DATE + 2, CURRENT_DATE + 2,
        'ACLS refresher', 'Rejected',
        (SELECT "Id" FROM "Staff" WHERE "Email"='admin@hospital.org'),
        NOW() - INTERVAL '5 days', 'Minimum staffing not met',
        NOW() - INTERVAL '6 days'
    ),
    (
        (SELECT "Id" FROM "Staff" WHERE "Email"='l.torres@hospital.org'),
        'Emergency', CURRENT_DATE + 7, CURRENT_DATE + 7,
        'Personal appointment', 'Pending', NULL, NULL, NULL,
        NOW() - INTERVAL '1 day'
    );

-- =============================================================================
-- LEAVE AUDIT ENTRIES
-- =============================================================================
INSERT INTO "LeaveAuditEntries" ("LeaveRequestId", "At", "By", "Action", "Note")
SELECT lr."Id", lr."SubmittedAt", s."FullName", 'submitted', NULL
FROM "LeaveRequests" lr
JOIN "Staff" s ON s."Id" = lr."StaffId";

INSERT INTO "LeaveAuditEntries" ("LeaveRequestId", "At", "By", "Action", "Note")
SELECT lr."Id", lr."ReviewedAt", rev."FullName", LOWER(lr."Status"), lr."ReviewNote"
FROM "LeaveRequests" lr
JOIN "Staff" rev ON rev."Id" = lr."ReviewedById"
WHERE lr."ReviewedById" IS NOT NULL;

-- =============================================================================
-- SHIFT SWAP REQUESTS
-- =============================================================================
INSERT INTO "ShiftSwapRequests"
    ("RequesterId", "RequesteeId", "RequesterShiftId", "RequesteeShiftId", "Reason", "Status", "SubmittedAt")
SELECT
    (SELECT "Id" FROM "Staff" WHERE "Email"='e.white@hospital.org'),
    (SELECT "Id" FROM "Staff" WHERE "Email"='a.johnson@hospital.org'),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'e.white@hospital.org' AND sh."ShiftType" = 'Morning'
     ORDER BY sh."StartTime" LIMIT 1),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'a.johnson@hospital.org' AND sh."ShiftType" = 'Afternoon'
     ORDER BY sh."StartTime" LIMIT 1),
    'Childcare conflict on Monday morning',
    'PendingRequestee',
    NOW() - INTERVAL '1 day';

INSERT INTO "ShiftSwapRequests"
    ("RequesterId", "RequesteeId", "RequesterShiftId", "RequesteeShiftId", "Reason", "Status", "SubmittedAt")
SELECT
    (SELECT "Id" FROM "Staff" WHERE "Email"='l.torres@hospital.org'),
    (SELECT "Id" FROM "Staff" WHERE "Email"='k.patel@hospital.org'),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'l.torres@hospital.org' AND sh."ShiftType" = 'Night'
     ORDER BY sh."StartTime" LIMIT 1),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'k.patel@hospital.org'
     ORDER BY sh."StartTime" LIMIT 1),
    'Medical appointment on Friday afternoon',
    'PendingAdmin',
    NOW() - INTERVAL '2 days';

-- =============================================================================
-- SWAP AUDIT ENTRIES
-- =============================================================================
INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt", 'Emma White', 'submitted', NULL
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Childcare%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt", 'Luis Torres', 'submitted', NULL
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Medical%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt" + INTERVAL '2 hours', 'Kira Patel', 'accepted', 'Happy to swap'
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Medical%';

-- Approved swap: Dr. James Park ↔ Dr. Priya Nair
INSERT INTO "ShiftSwapRequests"
    ("RequesterId", "RequesteeId", "RequesterShiftId", "RequesteeShiftId", "Reason", "Status", "SubmittedAt")
SELECT
    (SELECT "Id" FROM "Staff" WHERE "Email"='j.park@hospital.org'),
    (SELECT "Id" FROM "Staff" WHERE "Email"='p.nair@hospital.org'),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'j.park@hospital.org' AND sh."ShiftType" = 'Morning'
     ORDER BY sh."StartTime" LIMIT 1),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'p.nair@hospital.org' AND sh."ShiftType" = 'Morning'
     ORDER BY sh."StartTime" LIMIT 1),
    'Attending a conference that week',
    'Approved',
    NOW() - INTERVAL '5 days';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt", 'Dr. James Park', 'submitted', NULL
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Attending a conference%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt" + INTERVAL '3 hours', 'Dr. Priya Nair', 'accepted', 'Sure, I am free that day'
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Attending a conference%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt" + INTERVAL '10 hours', 'Admin User', 'approved', 'Approved, schedule updated'
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Attending a conference%';

-- Rejected swap: Diane Foster ↔ Dr. Marcus Kim (requestee declined)
INSERT INTO "ShiftSwapRequests"
    ("RequesterId", "RequesteeId", "RequesterShiftId", "RequesteeShiftId", "Reason", "Status", "SubmittedAt")
SELECT
    (SELECT "Id" FROM "Staff" WHERE "Email"='d.foster@hospital.org'),
    (SELECT "Id" FROM "Staff" WHERE "Email"='m.kim@hospital.org'),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'd.foster@hospital.org' AND sh."ShiftType" = 'Morning'
     ORDER BY sh."StartTime" LIMIT 1 OFFSET 1),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'm.kim@hospital.org' AND sh."ShiftType" = 'Morning'
     ORDER BY sh."StartTime" LIMIT 1 OFFSET 1),
    'Need Tuesday off for a personal matter',
    'Rejected',
    NOW() - INTERVAL '4 days';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt", 'Diane Foster', 'submitted', NULL
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Need Tuesday%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt" + INTERVAL '6 hours', 'Dr. Marcus Kim', 'rejected', 'Department meeting that morning, cannot swap'
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Need Tuesday%';

-- Cancelled swap: Dr. Sarah Chen ↔ Dr. Marcus Kim
INSERT INTO "ShiftSwapRequests"
    ("RequesterId", "RequesteeId", "RequesterShiftId", "RequesteeShiftId", "Reason", "Status", "SubmittedAt")
SELECT
    (SELECT "Id" FROM "Staff" WHERE "Email"='s.chen@hospital.org'),
    (SELECT "Id" FROM "Staff" WHERE "Email"='m.kim@hospital.org'),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 's.chen@hospital.org' AND sh."ShiftType" = 'Afternoon'
     ORDER BY sh."StartTime" LIMIT 1),
    (SELECT sh."Id" FROM "Shifts" sh
     JOIN "Staff" s ON s."Id" = sh."StaffId"
     WHERE s."Email" = 'm.kim@hospital.org' AND sh."ShiftType" = 'Afternoon'
     ORDER BY sh."StartTime" LIMIT 1),
    'Needed Wednesday afternoon for a family event',
    'Cancelled',
    NOW() - INTERVAL '3 days';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt", 'Dr. Sarah Chen', 'submitted', NULL
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Needed Wednesday%';

INSERT INTO "SwapAuditEntries" ("ShiftSwapRequestId", "At", "By", "Action", "Note")
SELECT "Id", "SubmittedAt" + INTERVAL '5 hours', 'Dr. Sarah Chen', 'cancelled', 'Plans changed, no longer needed'
FROM "ShiftSwapRequests" WHERE "Reason" LIKE 'Needed Wednesday%';

