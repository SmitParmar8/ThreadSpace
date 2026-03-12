// Create post page logic
renderNav();

// Require login
if (!isLoggedIn()) {
  window.location.href = 'login.html';
}

const titleInput = document.getElementById('title') as HTMLInputElement;
const titleCount = document.getElementById('titleCount')!;
const tagSelect = document.getElementById('tag') as HTMLSelectElement;
const cpErrorMsg = document.getElementById('errorMsg')!;

// Character counter
titleInput.addEventListener('input', () => {
  titleCount.textContent = `${titleInput.value.length} / 100`;
});

// Load tags
apiFetch('/api/tags').then((tags: any[]) => {
  tagSelect.innerHTML = tags.map(t =>
    `<option value="${t.id}">${t.name}</option>`
  ).join('');
}).catch(() => {
  tagSelect.innerHTML = '<option value="">Failed to load tags</option>';
});

// Form submit
const createPostForm = document.getElementById('createPostForm') as HTMLFormElement;
createPostForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  cpErrorMsg.textContent = '';

  const title = titleInput.value.trim();
  const content = (document.getElementById('content') as HTMLTextAreaElement).value.trim();
  const tagId = parseInt(tagSelect.value);
  const submitBtn = createPostForm.querySelector('button[type="submit"]') as HTMLButtonElement;

  if (!title) { cpErrorMsg.textContent = 'Title is required.'; return; }
  if (!content) { cpErrorMsg.textContent = 'Content is required.'; return; }
  if (!tagId) { cpErrorMsg.textContent = 'Please select a tag.'; return; }

  submitBtn.disabled = true;
  submitBtn.textContent = 'Publishing...';

  try {
    await apiFetch('/api/posts', {
      method: 'POST',
      body: JSON.stringify({ title, content, tagId })
    });
    window.location.href = 'index.html';
  } catch (err: any) {
    cpErrorMsg.textContent = err.message || 'Failed to create post.';
    submitBtn.disabled = false;
    submitBtn.textContent = 'Publish Post';
  }
});
