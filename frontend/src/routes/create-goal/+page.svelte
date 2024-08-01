<script>
    import { onMount } from 'svelte';
    let name = '';
    let description = '';
    let isSubmitting = false;
    let errorMessage = '';

    async function createGoal() {
        isSubmitting = true;
        errorMessage = '';

        try {
            const response = await fetch('https://localhost:5295/api/goal', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    name,
                    description,
                }),
            });

            if (response.ok) {
                // Handle successful goal creation, e.g., redirect or display success message
                alert('Goal created successfully!');
                name = '';
                description = '';
            } else {
                const errorData = await response.json();
                errorMessage = errorData.message || 'Failed to create goal.';
            }
        } catch (error) {
            console.error('Error:', error);
            errorMessage = 'An unexpected error occurred.';
        } finally {
            isSubmitting = false;
        }
    }
</script>

<style>
    .container {
        max-width: 600px;
        margin: auto;
        padding: 1rem;
    }

    .form-group {
        margin-bottom: 1rem;
    }

    .form-group label {
        display: block;
        margin-bottom: 0.5rem;
    }

    .form-group input,
    .form-group textarea {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #ddd;
        border-radius: 4px;
    }

    .form-group button {
        background-color: #4CAF50;
        color: white;
        padding: 0.5rem 1rem;
        border: none;
        border-radius: 4px;
        cursor: pointer;
    }

    .form-group button:disabled {
        background-color: #aaa;
        cursor: not-allowed;
    }
</style>

<div class="container">
    <h1 class="text-2xl font-bold mb-4">Create a New Goal</h1>
    {#if errorMessage}
        <div class="mb-4 text-red-500">{errorMessage}</div>
    {/if}
    <div class="form-group">
        <label for="name">Goal Name</label>
        <input id="name" type="text" bind:value={name} placeholder="Enter goal name" required />
    </div>
    <div class="form-group">
        <label for="description">Description</label>
        <textarea id="description" rows="4" bind:value={description} placeholder="Enter goal description" required></textarea>
    </div>
    <div class="form-group">
        <button on:click={createGoal} disabled={isSubmitting}>
            {#if isSubmitting}Creating...{/if}
            {#if !isSubmitting}Create Goal{/if}
        </button>
    </div>
</div>
