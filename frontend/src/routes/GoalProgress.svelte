<script>
    import { onMount } from 'svelte';

    let goals = [];

    onMount(async () => {
        try {
            const response = await fetch('https://localhost:7074/api/goal');
            if (response.ok) {
                goals = await response.json();
            } else {
                console.error('Failed to fetch goals:', response.statusText);
            }
        } catch (error) {
            console.error('Error fetching goals:', error);
        }
    });
</script>

<div>
    <h1 class="text-2xl font-bold">Goals</h1>
    <ul>
        {#each goals as goal}
            <li>{goal.name}</li>
        {/each}
    </ul>
</div>
