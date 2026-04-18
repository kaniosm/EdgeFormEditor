# EdgeFormEditor

`EdgeFormEditor` is a lightweight Windows Forms utility for viewing and managing Microsoft Edge autofill entries from the local `Web Data` SQLite database.

## Features

- Filter autofill records by `Name` and `Value`
- Sort grid columns by clicking headers
- Mark rows for deletion with red highlighting
- Commit pending deletions only when `Save` is clicked
- Startup warning with optional forced Edge shutdown to prevent lock/read-only errors

## Data Source

The app works with Edge’s local profile database at:

`%LOCALAPPDATA%\Microsoft\Edge\User Data\Default\Web Data`

## Tech Stack

- `.NET 10`
- `Windows Forms`
- `Entity Framework Core (SQLite)`

## Important Note

For reliable write operations, Microsoft Edge should be closed before saving changes, because running Edge processes can keep the database locked or effectively read-only.