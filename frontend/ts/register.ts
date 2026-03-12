// Register page logic
renderNav();

// Redirect if already logged in
if (isLoggedIn()) {
  window.location.href = 'index.html';
}

const registerForm = document.getElementById('registerForm') as HTMLFormElement;
const regErrorMsg = document.getElementById('errorMsg')!;
const regSuccessMsg = document.getElementById('successMsg')!;

registerForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  regErrorMsg.textContent = '';
  regSuccessMsg.textContent = '';

  const name = (document.getElementById('name') as HTMLInputElement).value.trim();
  const username = (document.getElementById('username') as HTMLInputElement).value.trim();
  const email = (document.getElementById('email') as HTMLInputElement).value.trim();
  const password = (document.getElementById('password') as HTMLInputElement).value;
  const userLevel = (document.getElementById('userLevel') as HTMLSelectElement).value;
  const submitBtn = registerForm.querySelector('button[type="submit"]') as HTMLButtonElement;

  if (password.length < 6) {
    regErrorMsg.textContent = 'Password must be at least 6 characters.';
    return;
  }

  submitBtn.disabled = true;
  submitBtn.textContent = 'Creating account...';

  try {
    const res = await apiFetch('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ name, username, email, password, userLevel })
    });
    setAuth(res.token, res.user);
    window.location.href = 'index.html';
  } catch (err: any) {
    regErrorMsg.textContent = err.message || 'Registration failed.';
    submitBtn.disabled = false;
    submitBtn.textContent = 'Create Account';
  }
});
