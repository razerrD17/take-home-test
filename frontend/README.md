# Frontend - Loan Management UI

An Angular 19 standalone application for displaying loan information, built with Angular Material.

## Architecture

The project uses **Angular 19 standalone components** (no traditional NgModules) with a simple, focused structure:

```
src/
├── app/
│   ├── models/
│   │   └── loan.model.ts        # TypeScript interfaces (Loan, CreateLoanDto, PaymentDto)
│   │
│   ├── services/
│   │   └── loan.service.ts      # HTTP service for API communication
│   │
│   ├── app.component.ts         # Main standalone component
│   ├── app.component.html       # Material table template
│   ├── app.component.scss       # Responsive styles
│   ├── app.config.ts            # Application configuration (providers)
│   └── app.routes.ts            # Routing configuration
│
├── environments/
│   ├── environment.ts           # Development config (localhost:5000)
│   └── environment.production.ts # Production config (/api)
│
├── styles.scss                  # Global Material Design styles
├── main.ts                      # Application bootstrap
└── index.html                   # HTML entry point
```

## Key Features

- **Standalone Components**: Modern Angular 19 architecture without NgModules
- **Angular Material**: Professional UI with Material Design components
- **Responsive Design**: 60% width on desktop, 100% on mobile
- **Loading State**: Spinner while fetching data
- **Error Handling**: User-friendly error messages with retry functionality
- **Environment Configuration**: Separate configs for development and production

## Models

```typescript
interface Loan {
  id: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: string;
}

interface CreateLoanDto {
  amount: number;
  applicantName: string;
}

interface PaymentDto {
  amount: number;
}
```

## Loan Service

The `LoanService` provides HTTP methods to communicate with the backend API:

| Method | Parameters | Return Type | Endpoint |
|--------|-----------|-------------|----------|
| `getLoans()` | - | `Observable<Loan[]>` | GET `/loans` |
| `getLoan(id)` | `id: number` | `Observable<Loan>` | GET `/loans/{id}` |
| `createLoan(loan)` | `CreateLoanDto` | `Observable<Loan>` | POST `/loans` |
| `makePayment(id, payment)` | `id, PaymentDto` | `Observable<Loan>` | POST `/loans/{id}/payment` |

## Prerequisites

- Node.js 20.x or later
- npm 10.x or later

## Running Locally

### 1. Install Dependencies

```sh
npm install
```

### 2. Start Development Server

```sh
npm start
```

Open `http://localhost:4200/` in your browser.

### 3. Backend Configuration

The frontend connects to the backend API. Configure the URL in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'  // Backend API URL
};
```

For production builds, the URL is configured in `environment.production.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: '/api'  // Relative path for reverse proxy
};
```

## Main Component Features

The `AppComponent` displays:

1. **Material Table**: Shows all loans with columns:
   - Loan Amount (formatted as currency)
   - Current Balance (formatted as currency)
   - Applicant Name
   - Status

2. **Loading State**: Displays a spinner with "Loading loans..." message while fetching data

3. **Error State**: Shows error message with a "Retry" button if the API call fails

4. **Responsive Layout**: Adapts to different screen sizes

## Building for Production

```sh
npm run build
```

Build output is stored in `dist/temp/browser/`.

**Build Budgets**:
- Initial bundle: max 500KB warning, 1MB error
- Component styles: max 4KB warning, 8KB error

## Running Tests

```sh
npm test
```

Uses Karma runner with Jasmine testing framework.

## Available Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start development server |
| `npm run build` | Build for production |
| `npm run watch` | Watch mode with dev config |
| `npm test` | Run unit tests |

## UI Styling

- **Font**: Roboto (Google Fonts)
- **Colors**:
  - Header text: `#7b8187` (gray)
  - Error messages: `#f44336` (red)
  - Borders: `#dee5ed` (light gray)
- **Layout**: Flexbox centered container
- **Table**: Material Design with rounded corners

## Technology Stack

- Angular 19.1
- Angular Material 19.2
- Angular CDK 19.2
- TypeScript 5.7
- RxJS 7.8
- SCSS
- Karma + Jasmine (Testing)

## Current Implementation Status

**Implemented**:
- Display loans in Material table
- Loading state with spinner
- Error handling with retry
- Responsive design
- Service methods for all CRUD operations

**Not Yet Implemented**:
- Authentication/Login UI
- Create loan form
- Payment form
- HTTP interceptors for auth tokens
- Route guards
