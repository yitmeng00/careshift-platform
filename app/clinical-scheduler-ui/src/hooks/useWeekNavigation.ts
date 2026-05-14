import { useState } from "react";

import { toISODate } from "../utils/dateUtils";

const getMondayOfWeek = (date: Date): Date => {
  const result = new Date(date);
  const daysFromMonday = (result.getDay() + 6) % 7;
  result.setDate(result.getDate() - daysFromMonday);
  result.setHours(0, 0, 0, 0);
  return result;
};

export const useWeekNavigation = () => {
  const [weekStart, setWeekStart] = useState<Date>(() =>
    getMondayOfWeek(new Date()),
  );

  const weekDays = Array.from({ length: 7 }, (_, i) => {
    const day = new Date(weekStart);
    day.setDate(weekStart.getDate() + i);
    return day;
  });

  const weekLabel = `${weekDays[0].toLocaleDateString("en-US", { month: "short", day: "numeric" })} - ${weekDays[6].toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}`;

  const nextWeek = () => {
    setWeekStart((prev) => {
      const next = new Date(prev);
      next.setDate(prev.getDate() + 7);
      return next;
    });
  };

  const prevWeek = () => {
    setWeekStart((prev) => {
      const next = new Date(prev);
      next.setDate(prev.getDate() - 7);
      return next;
    });
  };

  const goToToday = () => {
    setWeekStart(getMondayOfWeek(new Date()));
  };

  return {
    weekStart,
    weekDays,
    weekLabel,
    weekStartIso: toISODate(weekStart),
    nextWeek,
    prevWeek,
    goToToday,
  };
};
