import { getGoals } from '../lib/api'; // We'll create this later
import GoalGrid from '../components/GoalGrid';

export default async function Home() {
  const goals = await getGoals();

  return (
    <main className="container mx-auto p-4">
      <h1 className="text-3xl font-bold mb-4">My Goals</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {goals.map(goal => (
          <GoalGrid key={goal.id} goal={goal} />
        ))}
      </div>
    </main>
  );
}
