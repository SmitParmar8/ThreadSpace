// ===== CONFIG =====
const API_BASE = 'http://localhost:5001';
// ===== AUTH HELPERS =====
function getToken() {
    return localStorage.getItem('ts_token');
}
function getUser() {
    const raw = localStorage.getItem('ts_user');
    return raw ? JSON.parse(raw) : null;
}
function setAuth(token, user) {
    localStorage.setItem('ts_token', token);
    localStorage.setItem('ts_user', JSON.stringify(user));
}
function clearAuth() {
    localStorage.removeItem('ts_token');
    localStorage.removeItem('ts_user');
}
function isLoggedIn() {
    return !!getToken();
}
// ===== API FETCH =====
async function apiFetch(path, options = {}) {
    const token = getToken();
    const headers = { 'Content-Type': 'application/json' };
    if (token)
        headers['Authorization'] = `Bearer ${token}`;
    const res = await fetch(`${API_BASE}${path}`, {
        ...options,
        headers: { ...headers, ...(options.headers || {}) }
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({ message: res.statusText }));
        throw new Error(err.message || 'Request failed');
    }
    return res.json();
}
// ===== UTILITIES =====
function escapeHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}
function timeAgo(dateStr) {
    const diff = Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diff / 60000);
    if (mins < 1)
        return 'just now';
    if (mins < 60)
        return `${mins}m ago`;
    const hours = Math.floor(mins / 60);
    if (hours < 24)
        return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days < 30)
        return `${days}d ago`;
    return new Date(dateStr).toLocaleDateString();
}
function badgeClass(level) {
    const map = {
        'Student': 'badge-student',
        'Fresher': 'badge-fresher',
        'Mid-Level': 'badge-mid',
        'Pro': 'badge-pro'
    };
    return map[level] || 'badge-student';
}
// ===== NAVBAR =====
function renderNav() {
    const nav = document.getElementById('navbar');
    if (!nav)
        return;
    const user = getUser();
    nav.innerHTML = `
    <nav class="navbar">
      <div class="nav-inner">
        <a href="index.html" class="brand">&lt;ThreadSpace&gt;</a>
        <div class="nav-links">
          <a href="index.html">Feed</a>
          ${user ? `
            ${user.role === 'Admin' ? `<a href="admin.html" class="btn btn-outline admin-link">Admin</a>` : ''}
            <a href="profile.html?username=${user.username}" class="nav-user">
              <span class="badge badge-sm ${badgeClass(user.userLevel)}">${user.userLevel}</span>
              @${user.username}
            </a>
            <button id="logoutBtn" class="btn btn-outline">Logout</button>
          ` : `
            <a href="login.html" class="btn btn-outline">Login</a>
            <a href="register.html" class="btn btn-primary">Register</a>
          `}
        </div>
      </div>
    </nav>
  `;
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', async () => {
            try {
                await apiFetch('/api/auth/logout', { method: 'POST' });
            }
            catch (_) { }
            clearAuth();
            window.location.href = 'index.html';
        });
    }
}
