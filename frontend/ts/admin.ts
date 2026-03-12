// Admin dashboard logic
renderNav();

// Require admin
const adminUser = getUser();
if (!adminUser || adminUser.role !== 'Admin') {
  window.location.href = 'index.html';
}

const adminTagCreateError = document.getElementById('tagCreateError')!;
const adminTagCreateSuccess = document.getElementById('tagCreateSuccess')!;

async function loadStats(): Promise<void> {
  try {
    const stats = await apiFetch('/api/admin/stats');
    document.getElementById('statUsers')!.textContent = stats.totalUsers;
    document.getElementById('statPosts')!.textContent = stats.totalPosts;
    document.getElementById('statTags')!.textContent = stats.totalTags;
  } catch (err: any) {
    console.error('Failed to load stats:', err.message);
  }
}

async function loadAdminTags(): Promise<void> {
  const container = document.getElementById('tagsList')!;
  try {
    const tags = await apiFetch('/api/admin/tags');
    if (tags.length === 0) {
      container.innerHTML = '<div class="empty-state">No tags yet.</div>';
      return;
    }
    container.innerHTML = tags.map((t: any) => `
      <div class="tag-admin-row" id="tag-row-${t.id}">
        <div class="tag-admin-info">
          <span class="tag-admin-name">${escapeHtml(t.name)}</span>
          <span class="tag-admin-count">${t.postCount} post${t.postCount !== 1 ? 's' : ''}</span>
        </div>
        <button class="btn btn-danger btn-sm delete-tag-btn" data-id="${t.id}" data-posts="${t.postCount}">Delete</button>
      </div>
    `).join('');
    attachTagDeleteHandlers();
  } catch (err: any) {
    container.innerHTML = `<div class="empty-state">Failed to load tags.</div>`;
  }
}

function attachTagDeleteHandlers(): void {
  document.querySelectorAll('.delete-tag-btn').forEach(btn => {
    btn.addEventListener('click', async (e) => {
      const el = e.currentTarget as HTMLElement;
      const tagId = el.dataset.id!;
      const postCount = parseInt(el.dataset.posts!);
      if (postCount > 0) {
        alert(`Cannot delete — this tag is used by ${postCount} post${postCount !== 1 ? 's' : ''}.`);
        return;
      }
      if (!confirm('Delete this tag?')) return;
      try {
        await apiFetch(`/api/admin/tags/${tagId}`, { method: 'DELETE' });
        document.getElementById(`tag-row-${tagId}`)?.remove();
        loadStats();
      } catch (err: any) {
        alert(err.message);
      }
    });
  });
}

document.getElementById('createTagBtn')?.addEventListener('click', async () => {
  const input = document.getElementById('newTagInput') as HTMLInputElement;
  const name = input.value.trim();
  adminTagCreateError.textContent = '';
  adminTagCreateSuccess.textContent = '';

  if (!name) { adminTagCreateError.textContent = 'Tag name is required.'; return; }

  const btn = document.getElementById('createTagBtn') as HTMLButtonElement;
  btn.disabled = true;
  btn.textContent = 'Creating...';

  try {
    await apiFetch('/api/admin/tags', {
      method: 'POST',
      body: JSON.stringify({ name })
    });
    input.value = '';
    adminTagCreateSuccess.textContent = `Tag "${name}" created.`;
    loadAdminTags();
    loadStats();
  } catch (err: any) {
    adminTagCreateError.textContent = err.message;
  } finally {
    btn.disabled = false;
    btn.textContent = 'Create Tag';
  }
});

loadStats();
loadAdminTags();
