# Goals Tracker
Web app to track the progress of daily goals throughout the months.

![Goal Tracker](/images/goal-tracker.png)
![Goal Tracker](/images/goal-tracker1.png)

### Setup
Clone the repository:
`git clone https://github.com/borelli28/GoalTracker.git`

`cd GoalTracker`

### Backend Setup

Navigate to the backend directory:
`cd App`

Install .NET dependencies:
`dotnet restore`

Create the initial database migration:
`dotnet ef migrations add InitialCreate`

Apply the migration to create the database:
`dotnet ef database update`

Run the backend server:
`dotnet watch run`

The API should be running now

### Frontend Setup
Open a new terminal window and navigate to the frontend directory:
`cd ../frontend`

Install dependencies using Bun:
`bun install`

Create .env file with the backend API URL:
`echo VITE_API_URL=http://localhost:5295 > .env`

Start the Vite development server:
`bun run dev`

The frontend should now be accessible at http://localhost:5173 (or the port Vite assigns).
