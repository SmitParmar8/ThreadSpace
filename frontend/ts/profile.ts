// Profile page logic
renderNav();

const params = new URLSearchParams(window.location.search);
const username = params.get('username');

if (!username) {
  window.location.href = 'index.html';
}

function renderProfileCard(user: any): void {
  const profileCard = document.getElementById('profileCard')!;
  const initial = user.name ? user.name[0].toUpperCase() : user.username[0].toUpperCase();
  profileCard.innerHTML = `
    <div class="profile-card">
      <div class="profile-avatar">${initial}</div>
      <div>
        <div class="profile-name">${escapeHtml(user.name)}</div>
        <div class="profile-username">@${escapeHtml(user.username)}</div>
        <span class="badge ${badgeClass(user.userLevel)}">${user.userLevel}</span>
        <div class="profile-joined" style="margin-top:8px">Joined ${new Date(user.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'long' })}</div>
      </div>
    </div>
  `;
}

function renderProfilePosts(posts: any[]): void {
  const container = document.getElementById('profilePosts')!;
  const countEl = document.getElementById('profilePostCount')!;
  countEl.textContent = `${posts.length} post${posts.length !== 1 ? 's' : ''}`;

  if (posts.length === 0) {
    container.innerHTML = '<div class="empty-state">No posts yet.</div>';
    return;
  }

  container.innerHTML = posts.map(post => `
    <div class="post-card">
      <div class="post-header">
        <div class="post-meta">
          <span class="tag-chip">${escapeHtml(post.tag?.name || '')}</span>
        </div>
        <span class="post-timestamp">${timeAgo(post.createdAt)}</span>
      </div>
      <h3 class="post-title">${escapeHtml(post.title)}</h3>
      <p class="post-content">${escapeHtml(post.content)}</p>
      <div class="post-footer">
        <div class="vote-group">
          <span style="color:var(--text-muted);font-size:13px">Score:</span>
          <span class="vote-score">${post.voteScore}</span>
        </div>
      </div>
    </div>
  `).join('');
}

async function loadProfile(): Promise<void> {
  try {
    const [user, posts] = await Promise.all([
      apiFetch(`/api/users/${username}`),
      apiFetch(`/api/users/${username}/posts`)
    ]);
    document.title = `ThreadSpace – @${user.username}`;
    renderProfileCard(user);
    renderProfilePosts(posts);
  } catch (err: any) {
    document.getElementById('profileCard')!.innerHTML =
      `<div class="empty-state">User not found.</div>`;
    document.getElementById('profilePosts')!.innerHTML = '';
  }
}

loadProfile();
