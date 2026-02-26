// CFTClient Frontend JavaScript

const API_BASE = '/api';

// Toggle API Key visibility
function toggleApiKeyVisibility() {
    const apiKeyInput = document.getElementById('apiKey');
    const eyeIcon = document.getElementById('eyeIcon');

    if (apiKeyInput.type === 'password') {
        apiKeyInput.type = 'text';
        eyeIcon.classList.remove('bi-eye');
        eyeIcon.classList.add('bi-eye-slash');
    } else {
        apiKeyInput.type = 'password';
        eyeIcon.classList.remove('bi-eye-slash');
        eyeIcon.classList.add('bi-eye');
    }
}

// Get API Key from input
function getApiKey() {
    return document.getElementById('apiKey').value;
}

// Show loading spinner
function showLoading() {
    document.getElementById('loadingSpinner').classList.remove('d-none');
    document.getElementById('errorAlert').classList.add('d-none');
}

// Hide loading spinner
function hideLoading() {
    document.getElementById('loadingSpinner').classList.add('d-none');
}

// Show error message
function showError(message) {
    const errorAlert = document.getElementById('errorAlert');
    document.getElementById('errorMessage').textContent = message;
    errorAlert.classList.remove('d-none');
}

// API request helper
async function apiRequest(endpoint) {
    const response = await fetch(`${API_BASE}${endpoint}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-API-Key': getApiKey()
        }
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || 'حدث خطأ في الاتصال');
    }

    return await response.json();
}

// Format price
function formatPrice(price) {
    return new Intl.NumberFormat('ar-SA', {
        style: 'currency',
        currency: 'SAR'
    }).format(price);
}

// Render products in table
function renderProducts(products) {
    const tbody = document.getElementById('productsTable');
    const countBadge = document.getElementById('resultCount');

    countBadge.textContent = `${products.length} منتج`;

    if (products.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" class="text-center text-muted py-5">
                    <i class="bi bi-search display-4 d-block mb-2"></i>
                    لم يتم العثور على منتجات
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = products.map((product, index) => `
        <tr>
            <td>${index + 1}</td>
            <td><code>${product.productCode}</code></td>
            <td>${product.productName1 || ''}</td>
            <td dir="ltr" class="text-muted">${product.productName2 || ''}</td>
            <td class="price">${formatPrice(product.costValue)}</td>
            <td class="price text-success">${product.sellingPrice != null ? formatPrice(product.sellingPrice) : 'غير متوفر'}</td>
        </tr>
    `).join('');
}

// Search products
async function searchProducts() {
    const query = document.getElementById('searchQuery').value.trim();

    if (!query) {
        showError('الرجاء إدخال كلمة للبحث');
        return;
    }

    showLoading();

    try {
        const result = await apiRequest(`/products/search?q=${encodeURIComponent(query)}`);
        renderProducts(result.data);
    } catch (error) {
        showError(error.message);
        renderProducts([]);
    } finally {
        hideLoading();
    }
}

// Get all products
async function getAllProducts() {
    showLoading();

    try {
        const result = await apiRequest('/products');
        renderProducts(result.data);
    } catch (error) {
        showError(error.message);
        renderProducts([]);
    } finally {
        hideLoading();
    }
}

// Search on Enter key
document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchQuery');

    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            searchProducts();
        }
    });
});
