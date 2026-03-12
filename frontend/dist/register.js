// Register page logic
renderNav();
// Redirect if already logged in
if (isLoggedIn()) {
    window.location.href = 'index.html';
}
const registerForm = document.getElementById('registerForm');
const regErrorMsg = document.getElementById('errorMsg');
const regSuccessMsg = document.getElementById('successMsg');
registerForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    regErrorMsg.textContent = '';
    regSuccessMsg.textContent = '';
    const name = document.getElementById('name').value.trim();
    const username = document.getElementById('username').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    const userLevel = document.getElementById('userLevel').value;
    const submitBtn = registerForm.querySelector('button[type="submit"]');
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
    }
    catch (err) {
        regErrorMsg.textContent = err.message || 'Registration failed.';
        submitBtn.disabled = false;
        submitBtn.textContent = 'Create Account';
    }
});
