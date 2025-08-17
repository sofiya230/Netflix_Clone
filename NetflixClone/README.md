# NetflixClone - Full Stack Netflix Clone

A complete Netflix clone built with React frontend and .NET Core backend, featuring user authentication, content management, watch history tracking, and a responsive Netflix-like interface.

## 🚀 Features

- **User Authentication**: JWT-based login/register with 2FA support
- **Profile Management**: Multiple user profiles with custom avatars
- **Content Management**: Movies and TV series with episode support
- **Watch History**: Track viewing progress and continue watching
- **My List**: Save favorite content for later viewing
- **Admin Panel**: Content and episode management for administrators
- **Responsive Design**: Mobile-first Netflix-like interface

## 🛠️ Tech Stack

### Frontend
- React 18 with Vite
- Tailwind CSS for styling
- React Router for navigation
- Context API for state management

### Backend
- .NET 8 Core Web API
- Entity Framework Core with MySQL
- JWT Authentication
- Email services (Gmail API, Resend)

## 📋 Prerequisites

- .NET 8 SDK
- Node.js 18+
- MySQL Server
- Gmail account (for email functionality)

## ⚙️ Setup Instructions

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd NetflixClone
```

### 2. Backend Setup
```bash
cd NetflixClone
# Copy the template configuration
cp appsettings.template.json appsettings.json
# Edit appsettings.json with your credentials
```

**Configure `appsettings.json`:**
- Update database connection string with your MySQL credentials
- Generate a secure JWT key (at least 32 characters)
- Add your Gmail credentials for email functionality

### 3. Database Setup
```bash
# Run migrations
dotnet ef database update
```

### 4. Frontend Setup
```bash
cd Netflix_Frontend
npm install
npm run dev
```

### 5. Run the Application
```bash
# Backend (from NetflixClone directory)
dotnet run

# Frontend (from Netflix_Frontend directory)
npm run dev
```

## 🔐 Environment Variables

Create a `.env` file in the frontend directory:
```env
VITE_API_BASE_URL=http://localhost:5000/api
```

## 📱 Usage

1. **Register/Login**: Create an account or sign in
2. **Select Profile**: Choose or create a viewing profile
3. **Browse Content**: Explore movies and TV series
4. **Watch Content**: Click play to start watching
5. **Manage Lists**: Add/remove content from My List
6. **Admin Panel**: Manage content and episodes (Admin role required)

## 🗄️ Database Schema

The application uses Entity Framework Core with the following main entities:
- Users and Profiles
- Content (Movies/TV Shows)
- Episodes
- Watch History
- My List



