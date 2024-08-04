# Goals Tracker
Web app to track progress of daily goals throught the months.

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

The API should now be running now

### Frontend Setup
Open a new terminal window and navigate to the frontend directory:
`cd ../frontend`

Install Node.js dependencies:
`npm install`

Start the React development server:
`npm start`

The frontend should now be accessible at http://localhost:3000.
