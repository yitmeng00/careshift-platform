import { z } from "zod";

export const createShiftSchema = z.object({
  staffId: z.string().min(1, "Select a staff member"),
  departmentId: z.string(),
  date: z
    .string()
    .min(1, "Select a date")
    .refine(
      (val) => {
        const d = new Date();
        const today = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
        return val >= today;
      },
      { message: "Date cannot be in the past" },
    ),
  shiftType: z.enum(["Morning", "Afternoon", "Night"]),
  notes: z.string().optional(),
});
