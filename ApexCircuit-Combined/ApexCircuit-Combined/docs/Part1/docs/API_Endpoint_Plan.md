# SECTION B — API Endpoint Plan
## RaceDay Event Management System

---

### Authentication Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | Registers a new user with role assignment | None (public) | `{ firstName, lastName, email, password, roleName }` | `201 Created` — User object with token. `400` — Invalid data. `409` — Email exists. |
| POST | `/api/auth/login` | Authenticates user and returns JWT token | None (public) | `{ email, password }` | `200 OK` — JWT token + user details. `401` — Invalid credentials. |

### User Profile Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/users/profile` | Retrieves logged-in user's profile | Any (logged in) | None | `200 OK` — User profile. `401` — Not logged in. |
| PUT | `/api/users/profile` | Updates user's profile | Any (logged in) | `{ firstName, lastName, email, passwordHash }` | `200 OK` — Updated profile. `400` — Invalid data. |
| GET | `/api/users/{id}/enrolments` | Gets all enrolments for a user | Any (own only) | None | `200 OK` — List of enrolments. `403` — Accessing other's data. |
| GET | `/api/users/{id}/results` | Gets all race results for a user | Any (own only) | None | `200 OK` — List of results. `403` — Accessing other's data. |

### Event Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events` | Lists all upcoming events | None (public) | None | `200 OK` — Array of events. `404` — No events. |
| GET | `/api/events/{id}` | Gets details for a specific event | None (public) | None | `200 OK` — Event + categories. `404` — Not found. |
| POST | `/api/events` | Creates a new event | Organiser | `{ eventName, description, eventDate, location, maxEntrants, postCode }` | `201 Created` — New event. `400` — Missing fields. `403` — Not organiser. |
| PUT | `/api/events/{id}` | Updates an existing event | Organiser (owner) | `{ eventName, description, eventDate, location, maxEntrants, postCode }` | `200 OK` — Updated event. `403` — Not owner. `404` — Not found. |
| DELETE | `/api/events/{id}` | Deletes an event | Organiser (owner) | None | `204 No Content`. `403` — Not owner. `404` — Not found. |
| GET | `/api/events/organiser/{id}` | Gets all events by an organiser | Organiser | None | `200 OK` — Array of events. `403` — Not the organiser. |

### Category Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | `/api/events/{id}/categories` | Gets all categories for an event | None (public) | None | `200 OK` — Array of categories. `404` — Event not found. |
| POST | `/api/events/{id}/categories` | Adds a category to an event | Organiser (owner) | `{ categoryName, description, entryFee }` | `201 Created` — New category. `400` — Invalid data. `403` — Not owner. |
| PUT | `/api/categories/{id}` | Updates a category | Organiser (owner) | `{ categoryName, description, entryFee }` | `200 OK` — Updated category. `403` — Not owner. `404` — Not found. |
| DELETE | `/api/categories/{id}` | Deletes a category | Organiser (owner) | None | `204 No Content`. `403` — Not owner. |

### Enrolment Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/enrolments` | Registers participant into a category | Participant | `{ categoryId, eventId, enrolmentDate }` | `201 Created` — Enrolment. `409` — Already enrolled. `403` — Not participant. |
| GET | `/api/enrolments` | Gets all enrolments (filtered by role) | Any (logged in) | None | `200 OK` — Array of enrolments. |
| GET | `/api/enrolments/{id}` | Gets a specific enrolment | Any (owner or organiser) | None | `200 OK` — Enrolment details. `403` — No access. `404` — Not found. |
| PUT | `/api/enrolments/{id}/status` | Updates enrolment status | Organiser | `{ status }` | `200 OK` — Updated. `403` — Not organiser. |
| DELETE | `/api/enrolments/{id}` | Cancels an enrolment | Participant (own) or Organiser | None | `204 No Content`. `403` — No access. |

### Results Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/results` | Records a race result | Organiser | `{ enrolmentId, finishTime, position, resultStatus }` | `201 Created` — Result. `400` — Invalid data. `403` — Not organiser. |
| GET | `/api/results` | Gets results (filtered by role) | Any (logged in) | None | `200 OK` — Array of results. |
| GET | `/api/results/{id}` | Gets a specific result | Any (owner or organiser) | None | `200 OK` — Result details. `403` — No access. `404` — Not found. |
| GET | `/api/events/{id}/results` | Gets leaderboard for an event | None (public) | None | `200 OK` — Results sorted by position. `404` — Event not found. |
| PUT | `/api/results/{id}` | Updates a result | Organiser | `{ finishTime, position, resultStatus }` | `200 OK` — Updated. `403` — Not organiser. `404` — Not found. |

---

**Total Endpoints Planned: 22**
**Coverage: Authentication (2), Users (4), Events (6), Categories (4), Enrolments (5), Results (5)**
