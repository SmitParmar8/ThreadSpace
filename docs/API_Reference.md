Phase - 1

1. Authentication
| Method | Route                | Description                            | Access        |
| ------ | -------------------- | -------------------------------------- | ------------- |
| POST   | `/api/auth/register` | Register a new user                    | Public        |
| POST   | `/api/auth/login`    | Login and receive JWT token            | Public        |
| POST   | `/api/auth/logout`   | Invalidate user session                | Authenticated |

2. User Profile
| Method | Route                        | Description                     | Access        |
| ------ | ---------------------------- | ------------------------------- | ------------- |
| GET    | `/api/users/:username`       | Get user profile by username    | Public        |
| GET    | `/api/users/:username/posts` | Get posts created by this user  | Public        |

3. Posts
| Method | Route            | Description                          | Access        |
| ------ | ---------------- | ------------------------------------ | ------------- |
| GET    | `/api/posts`     | Feed — list all posts (newest first) | Public        |
| POST   | `/api/posts`     | Create a new post                    | Authenticated |
| PUT    | `/api/posts/:id` | Update user's own post               | Authenticated |
| DELETE | `/api/posts/:id` | Delete user's own post               | Authenticated |

4. Voting
| Method | Route                 | Description               | Access        |
| ------ | --------------------- | ------------------------- | ------------- |
| POST   | `/api/posts/:id/vote` | Upvote or downvote a post | Authenticated |
| DELETE | `/api/posts/:id/vote` | Remove user's vote        | Authenticated |

5. Tags
| Method | Route                 | Description               | Access |
| ------ | --------------------- | ------------------------- | ------ |
| GET    | `/api/tags`           | List all available tags   | Public |
| GET    | `/api/tags/:id/posts` | Get posts filtered by tag | Public |

6. AI Feed
| Method | Route               | Description                    | Access |
| ------ | ------------------- | ------------------------------ | ------ |
| GET    | `/api/ai-posts`     | AI-generated posts feed        | Public |


