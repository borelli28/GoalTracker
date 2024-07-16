import ProgressGrid from './ProgressGrid';

export default function GoalGrid({ goal }) {
  return (
    <div className="border rounded-lg p-4 shadow-md">
      <h2 className="text-xl font-semibold mb-2">{goal.name}</h2>
      <p className="text-gray-600 mb-4">{goal.description}</p>
      <ProgressGrid progress={goal.progress} />
    </div>
  );
}
