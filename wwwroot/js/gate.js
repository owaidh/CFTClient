// ── API Key Gate ──────────────────────────────────────────
// Manages the fullscreen overlay that asks for the API key
// on first visit (or after logout / key cleared).
// ---------------------------------------------------------

const STORAGE_KEY = 'cft_api_key';

// Called once from app.js — shows gate if no saved key
function initGate() {
    const saved = localStorage.getItem(STORAGE_KEY) || sessionStorage.getItem(STORAGE_KEY);
    if (saved) {
        // Key exists — dismiss gate immediately (no animation needed)
        dismissGate(false);
    } else {
        // Show gate and focus input
        const gate = document.getElementById('apiKeyGate');
        gate.classList.remove('hide');
        setTimeout(() => document.getElementById('gateApiKeyInput')?.focus(), 100);
    }

    // Allow submitting with Enter key
    document.getElementById('gateApiKeyInput')?.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') submitGateKey();
    });
}

// Toggle password visibility inside the gate
function toggleGateVisibility() {
    const input = document.getElementById('gateApiKeyInput');
    const icon  = document.getElementById('gateEyeIcon');
    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';
    icon.classList.toggle('bi-eye',       !isPassword);
    icon.classList.toggle('bi-eye-slash',  isPassword);
}

// Validate key against the server then dismiss
async function submitGateKey() {
    const input  = document.getElementById('gateApiKeyInput');
    const errEl  = document.getElementById('gateError');
    const btn    = document.getElementById('gateSubmitBtn');
    const remember = document.getElementById('gateRemember').checked;
    const key    = input.value.trim();

    if (!key) {
        shake(input);
        errEl.textContent = 'يرجى إدخال مفتاح API';
        return;
    }

    // Disable button while validating
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> جاري التحقق...';
    errEl.textContent = '';

    try {
        const res = await fetch('/api/products?_validate=1&_limit=0', {
            method: 'GET',
            headers: { 'X-API-Key': key }
        });

        if (res.status === 401) {
            errEl.textContent = 'المفتاح غير صحيح، يرجى المحاولة مرة أخرى';
            shake(input);
            input.select();
            return;
        }

        // Key is valid — save it
        if (remember) {
            localStorage.setItem(STORAGE_KEY, key);
        } else {
            sessionStorage.setItem(STORAGE_KEY, key);
        }

        dismissGate(true);

    } catch {
        errEl.textContent = 'تعذّر الاتصال بالخادم';
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-unlock-fill me-1"></i> دخول';
    }
}

// Animate the gate out
function dismissGate(animate = true) {
    const gate = document.getElementById('apiKeyGate');
    if (animate) {
        gate.classList.add('hide');
        setTimeout(() => gate.remove(), 450);
    } else {
        gate.remove();
    }
}

// Shake animation for wrong input
function shake(el) {
    el.style.animation = 'none';
    el.offsetHeight; // reflow
    el.style.animation = 'gateShake 0.4s ease';
    el.addEventListener('animationend', () => el.style.animation = '', { once: true });
}

// Inject keyframe for shake if not already present
(function injectShakeKeyframe() {
    if (document.getElementById('gate-keyframes')) return;
    const style = document.createElement('style');
    style.id = 'gate-keyframes';
    style.textContent = `
        @keyframes gateShake {
            0%,100% { transform: translateX(0); }
            20%      { transform: translateX(-8px); }
            40%      { transform: translateX(8px); }
            60%      { transform: translateX(-5px); }
            80%      { transform: translateX(5px); }
        }
    `;
    document.head.appendChild(style);
})();

// Return the currently stored API key (used by app.js)
function getStoredApiKey() {
    return localStorage.getItem(STORAGE_KEY) || sessionStorage.getItem(STORAGE_KEY) || '';
}

// Bootstrap on DOM ready
document.addEventListener('DOMContentLoaded', initGate);
