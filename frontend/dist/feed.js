// Feed page logic
let allPosts = [];
let allTags = [];
let activeTagId = null;
let editingPostId = null;
function renderPostCard(post) {
    const user = getUser();
    const isOwner = user && post.author && user.username === post.author.username;
    const level = post.author?.userLevel || 'Student';
    return `
    <div class="post-card" id="post-${post.id}">
      <div class="post-header">
        <div class="post-meta">
          <a href="profile.html?username=${post.author?.username}" class="post-author">@${post.author?.username}</a>
          <span class="badge ${badgeClass(level)}">${level}</span>
          <span class="tag-chip" data-tag="${post.tag?.id}">${post.tag?.name}</span>
        </div>
        <span class="post-timestamp">${timeAgo(post.createdAt)}</span>
      </div>
      <h3 class="post-title">${escapeHtml(post.title)}</h3>
      <p class="post-content">${escapeHtml(post.content)}</p>
      <div class="post-footer">
        <div class="vote-group">
          <button class="vote-btn upvote" data-id="${post.id}" data-like="true" title="Upvote">▲</button>
          <span class="vote-score" id="score-${post.id}">${post.voteScore}</span>
          <button class="vote-btn downvote" data-id="${post.id}" data-like="false" title="Downvote">▼</button>
        </div>
        ${isOwner ? `
          <div class="post-actions">
            <button class="btn btn-outline btn-sm edit-btn" data-id="${post.id}">Edit</button>
            <button class="btn btn-danger btn-sm delete-btn" data-id="${post.id}">Delete</button>
          </div>
        ` : ''}
      </div>
    </div>
  `;
}
function renderPosts(posts) {
    const container = document.getElementById('postsList');
    const countEl = document.getElementById('postCount');
    if (posts.length === 0) {
        container.innerHTML = '<div class="empty-state">No posts yet. Be the first to post!</div>';
        countEl.textContent = '';
        return;
    }
    countEl.textContent = `${posts.length} post${posts.length !== 1 ? 's' : ''}`;
    container.innerHTML = posts.map(renderPostCard).join('');
    attachPostHandlers();
}
function renderTagFilters(tags) {
    const container = document.getElementById('tagFilters');
    const allBtn = `<button class="tag-filter-btn active" data-tag-id="null">All</button>`;
    const tagBtns = tags.map(t => `<button class="tag-filter-btn" data-tag-id="${t.id}">${t.name}</button>`).join('');
    container.innerHTML = allBtn + tagBtns;
    container.addEventListener('click', (e) => {
        const btn = e.target.closest('.tag-filter-btn');
        if (!btn)
            return;
        document.querySelectorAll('.tag-filter-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        const tagId = btn.dataset.tagId;
        activeTagId = tagId === 'null' ? null : parseInt(tagId);
        const filtered = activeTagId ? allPosts.filter(p => p.tag?.id === activeTagId) : allPosts;
        renderPosts(filtered);
    });
}
function renderAiPosts(posts) {
    const container = document.getElementById('aiPostsList');
    if (posts.length === 0) {
        container.innerHTML = '<div class="empty-state">No AI posts.</div>';
        return;
    }
    container.innerHTML = posts.map(p => `
    <div class="ai-post-card">
      <div class="ai-badge">AI · ${escapeHtml(p.aiModel)}</div>
      <div class="ai-post-title">${escapeHtml(p.title)}</div>
      <div class="ai-post-content-full">${escapeHtml(p.content)}</div>
      <div class="ai-post-time">${timeAgo(p.createdAt)}</div>
    </div>
  `).join('');
}
function attachPostHandlers() {
    // Vote buttons
    document.querySelectorAll('.vote-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            if (!isLoggedIn()) {
                window.location.href = 'login.html';
                return;
            }
            const el = e.currentTarget;
            const postId = el.dataset.id;
            const isLike = el.dataset.like === 'true';
            try {
                const res = await apiFetch(`/api/posts/${postId}/vote`, {
                    method: 'POST',
                    body: JSON.stringify({ isLike })
                });
                const scoreEl = document.getElementById(`score-${postId}`);
                if (scoreEl)
                    scoreEl.textContent = res.voteScore;
            }
            catch (err) {
                console.error(err.message);
            }
        });
    });
    // Tag chip filter
    document.querySelectorAll('.tag-chip').forEach(chip => {
        chip.addEventListener('click', (e) => {
            const tagId = parseInt(e.currentTarget.dataset.tag);
            document.querySelectorAll('.tag-filter-btn').forEach(b => b.classList.remove('active'));
            const btn = document.querySelector(`.tag-filter-btn[data-tag-id="${tagId}"]`);
            if (btn)
                btn.classList.add('active');
            activeTagId = tagId;
            renderPosts(allPosts.filter(p => p.tag?.id === tagId));
        });
    });
    // Edit button
    document.querySelectorAll('.edit-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const postId = parseInt(e.currentTarget.dataset.id);
            openEditModal(postId);
        });
    });
    // Delete button
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            const postId = e.currentTarget.dataset.id;
            if (!confirm('Delete this post?'))
                return;
            try {
                await apiFetch(`/api/posts/${postId}`, { method: 'DELETE' });
                allPosts = allPosts.filter(p => p.id !== parseInt(postId));
                const filtered = activeTagId ? allPosts.filter(p => p.tag?.id === activeTagId) : allPosts;
                renderPosts(filtered);
            }
            catch (err) {
                alert(err.message);
            }
        });
    });
}
function openEditModal(postId) {
    const post = allPosts.find(p => p.id === postId);
    if (!post)
        return;
    editingPostId = postId;
    document.getElementById('editTitle').value = post.title;
    document.getElementById('editContent').value = post.content;
    const tagSelect = document.getElementById('editTag');
    tagSelect.innerHTML = allTags.map(t => `<option value="${t.id}" ${t.id === post.tag?.id ? 'selected' : ''}>${t.name}</option>`).join('');
    document.getElementById('editError').textContent = '';
    document.getElementById('editModal').classList.remove('hidden');
}
async function loadFeed() {
    try {
        const [posts, tags, aiPosts] = await Promise.all([
            apiFetch('/api/posts'),
            apiFetch('/api/tags'),
            apiFetch('/api/ai-posts')
        ]);
        allPosts = posts;
        allTags = tags;
        renderTagFilters(tags);
        renderPosts(posts);
        renderAiPosts(aiPosts);
    }
    catch (err) {
        document.getElementById('postsList').innerHTML =
            `<div class="empty-state">Failed to load posts: ${err.message}</div>`;
    }
}
// ===== INIT =====
renderNav();
loadFeed();
// Show New Post button only on Feed tab for logged-in users
function updateNewPostBtn(activeTab) {
    const btn = document.getElementById('newPostBtn');
    if (!btn)
        return;
    if (activeTab === 'feed' && isLoggedIn()) {
        btn.classList.remove('hidden');
    }
    else {
        btn.classList.add('hidden');
    }
}
// Tab switching
document.querySelectorAll('.feed-tab').forEach(tab => {
    tab.addEventListener('click', () => {
        const target = tab.dataset.tab;
        document.querySelectorAll('.feed-tab').forEach(t => t.classList.remove('active'));
        tab.classList.add('active');
        document.getElementById('tabFeed').classList.toggle('hidden', target !== 'feed');
        document.getElementById('tabAi').classList.toggle('hidden', target !== 'ai');
        updateNewPostBtn(target);
    });
});
// Init New Post button visibility
updateNewPostBtn('feed');
// Edit modal handlers
document.getElementById('closeModal')?.addEventListener('click', () => {
    document.getElementById('editModal').classList.add('hidden');
});
document.getElementById('saveEditBtn')?.addEventListener('click', async () => {
    if (!editingPostId)
        return;
    const title = document.getElementById('editTitle').value.trim();
    const content = document.getElementById('editContent').value.trim();
    const tagId = parseInt(document.getElementById('editTag').value);
    const errEl = document.getElementById('editError');
    if (!title || !content) {
        errEl.textContent = 'Title and content are required.';
        return;
    }
    try {
        await apiFetch(`/api/posts/${editingPostId}`, {
            method: 'PUT',
            body: JSON.stringify({ title, content, tagId })
        });
        const idx = allPosts.findIndex(p => p.id === editingPostId);
        if (idx !== -1) {
            allPosts[idx].title = title;
            allPosts[idx].content = content;
            allPosts[idx].tag = allTags.find(t => t.id === tagId);
        }
        document.getElementById('editModal').classList.add('hidden');
        const filtered = activeTagId ? allPosts.filter(p => p.tag?.id === activeTagId) : allPosts;
        renderPosts(filtered);
    }
    catch (err) {
        errEl.textContent = err.message;
    }
});
// Close modal on backdrop click
document.getElementById('editModal')?.addEventListener('click', (e) => {
    if (e.target === document.getElementById('editModal')) {
        document.getElementById('editModal').classList.add('hidden');
    }
});
