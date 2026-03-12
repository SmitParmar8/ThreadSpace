Frontend must be static and lightweight so it can be easily hosted inside a Docker container.=
It should not require additional runtime environments.

Simple UI
HTML + CSS + typescript
REST API based data loading
JSON responses from backend
Minimal complexity
Docker compatible
Guest browsing allowed
Authentication required for actions
## Phase 1 Requirements

### 1. Authentication

**POST `/api/auth/register`**
- Accept: `name`, `username`, `email`, `password`
- Validate unique username and email
- Hash password before storing
- Return success message (redirect user to login)

**POST `/api/auth/login`**
- Accept: `email` or `username` + `password`
- Validate credentials, return signed JWT token
- Token should include `user_id`, `username`, `role`

**POST `/api/auth/logout`**
- Invalidate/clear the JWT session on client side
- Requires valid JWT in header

---

### 2. User Profile

**GET `/api/users/:username`**
- Return: `name`, `username`, `email`, `user_level` (badge), `created_at`
- Public — no auth needed

**GET `/api/users/:username/posts`**
- Return all posts created by that user
- Each post includes: `title`, `content`, `tag`, `vote_count`, `created_at`

---

### 3. Posts

**GET `/api/posts`**
- Return all posts sorted by newest first
- Each post includes: `title`, `content`, `author username`, `user_level badge`, `tag`, `vote_count`, `created_at`
- Supports pagination

**POST `/api/posts`**
- Auth required
- Accept: `title`, `content`, `tag_id`
- Title max 100 characters
- Validate `tag_id` exists

**PUT `/api/posts/:id`**
- Auth required, only post owner can edit
- Accept: `title`, `content`, `tag_id`
- Return 403 if user is not the owner

**DELETE `/api/posts/:id`**
- Auth required, only post owner can delete
- Return 403 if user is not the owner

---

### 4. Voting

**POST `/api/posts/:id/vote`**
- Auth required
- Accept: `{ "is_like": true | false }`
- Prevent duplicate votes — if same user votes again on same post, update the existing vote
- Recalculate and return updated vote count

**DELETE `/api/posts/:id/vote`**
- Auth required
- Remove the user's existing vote on that post
- Return 404 if no vote exists to remove

---

### 5. Tags

**GET `/api/tags`**
- Return all tags: `id`, `name`, `created_at`
- Public — no auth needed
- Used to populate tag selector on Create Post page

**GET `/api/tags/:id/posts`**
- Return all posts associated with that tag
- Same post structure as `GET /api/posts`

---

### 6. AI Feed

**GET `/api/ai-posts`**
- Return all AI-generated posts: `title`, `content`, `ai_model`, `created_at`
- Read-only — no create, edit, or delete
- Public — no auth needed

> Note: `GET /api/ai-posts/:id` is not in your Phase 1 route table — add it if you need a single AI post detail page, otherwise skip it.

---

### Cross-cutting Rules

- All protected routes validate JWT from `Authorization: Bearer <token>` header
- Ownership checks on edit/delete are server-side, not just frontend-gated
- Vote score = total upvotes − total downvotes, computed on read or stored as a counter