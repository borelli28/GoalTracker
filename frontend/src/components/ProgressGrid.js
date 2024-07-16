export default function ProgressGrid({ progress = [] }) {
  const weeks = 12; // Show 12 weeks of progress
  const days = 7;

  // Ensure progress is an array, even if it's undefined
  const safeProgress = Array.isArray(progress) ? progress : [];

  return (
    <div className="grid grid-cols-7 gap-1">
      {[...Array(weeks * days)].map((_, index) => {
        const day = safeProgress[index] || { completed: false };
        const bgColor = day.completed ? 'bg-green-500' : 'bg-gray-200';
        return (
          <div
            key={index}
            className={`w-4 h-4 ${bgColor} rounded-sm`}
            title={day.date ? `Date: ${day.date}, Notes: ${day.notes}` : 'No data'}
          />
        );
      })}
    </div>
  );
}
