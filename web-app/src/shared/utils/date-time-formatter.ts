export default function (dateTime: Date): string {
  const [weekday, date, time] = new Intl.DateTimeFormat("en-GB", {
    weekday: "short",
    hour: "2-digit",
    minute: "2-digit",
    year: "2-digit",
    month: "2-digit",
    day: "2-digit",
    hour12: true,
  })
    .format(dateTime)
    .toLowerCase()
    .split(",");
  return `${weekday} ${date} | ${time}`;
}
