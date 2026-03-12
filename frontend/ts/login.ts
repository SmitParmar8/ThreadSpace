// Login page logic
renderNav();

// Redirect if already logged in
if (isLoggedIn()) {
  window.location.href = 'index.html';
}

const loginForm = document.getElementById('loginForm') as HTMLFormElement;
const loginErrorMsg = document.getElementById('errorMsg')!;

loginForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  loginErrorMsg.textContent = '';

  const email = (document.getElementById('email') as HTMLInputElement).value.trim();
  const password = (document.getElementById('password') as HTMLInputElement).value;
  const submitBtn = loginForm.querySelector('button[type="submit"]') as HTMLButtonElement;

  submitBtn.disabled = true;
  submitBtn.textContent = 'Signing in...';

  try {
    const res = await apiFetch('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });
    setAuth(res.token, res.user);
    window.location.href = 'index.html';
  } catch (err: any) {
    loginErrorMsg.textContent = err.message || 'Login failed. Check your credentials.';
    submitBtn.disabled = false;
    submitBtn.textContent = 'Sign In';
  }
});
