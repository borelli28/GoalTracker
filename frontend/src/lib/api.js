export async function getGoals() {
  const res = await fetch('http://localhost:5295/api/Goal');
  if (!res.ok) {
    throw new Error('Failed to fetch goals');
  }
  return res.json();
}
