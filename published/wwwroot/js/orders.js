// Orders Management JavaScript
// Handles AJAX operations for order status updates and interactive features

// Global variables
let currentOrderId = null;

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function() {
    initializeTooltips();
    initializeDataTables();
    bindEventHandlers();
});

// Initialize Bootstrap tooltips
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Initialize DataTables for better table functionality
function initializeDataTables() {
    // Only initialize if DataTables library is available
    if (typeof $.fn.DataTable !== 'undefined') {
        $('#ordersTable').DataTable({
            "pageLength": 25,
            "order": [[ 6, "desc" ]], // Sort by date column descending
            "columnDefs": [
                { "orderable": false, "targets": 8 } // Disable sorting for actions column
            ],
            "language": {
                "search": "Search orders:",
                "lengthMenu": "Show _MENU_ orders per page",
                "info": "Showing _START_ to _END_ of _TOTAL_ orders",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                }
            }
        });
    }
}

// Bind event handlers
function bindEventHandlers() {
    // Handle form submission for filters
    const filterForm = document.querySelector('form[action*="Index"]');
    if (filterForm) {
        filterForm.addEventListener('submit', function(e) {
            showLoadingIndicator();
        });
    }

    // Handle quick filter badges
    document.querySelectorAll('.badge[href]').forEach(badge => {
        badge.addEventListener('click', function() {
            showLoadingIndicator();
        });
    });
}

// Open status update modal
function openStatusUpdateModal(orderId, trackingNumber, currentStatus, customerName) {
    currentOrderId = orderId;
    
    // Populate modal fields
    document.getElementById('orderId').value = orderId;
    document.getElementById('orderInfo').textContent = `Order #${orderId} - ${trackingNumber} (${customerName})`;
    document.getElementById('currentStatus').innerHTML = `<span class="badge ${getStatusBadgeClass(currentStatus)}">${currentStatus}</span>`;
    
    // Reset new status dropdown
    document.getElementById('newStatus').value = '';
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('statusUpdateModal'));
    modal.show();
}

// Update order status via AJAX
function updateOrderStatus() {
    const orderId = document.getElementById('orderId').value;
    const newStatus = document.getElementById('newStatus').value;
    const statusChangeReason = document.getElementById('statusChangeReason')?.value || '';
    
    if (!orderId || !newStatus) {
        showAlert('Please select a new status', 'warning');
        return;
    }
    
    // Show loading state
    const updateButton = document.querySelector('#statusUpdateModal .btn-primary');
    const originalText = updateButton.innerHTML;
    updateButton.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Updating...';
    updateButton.disabled = true;
    
    // Prepare request data
    const requestData = {
        OrderId: parseInt(orderId),
        NewStatus: parseInt(newStatus),
        Reason: statusChangeReason
    };
    
    // Make AJAX request
    fetch('/Orders/UpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        body: JSON.stringify(requestData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Update the status badge in the table
            updateStatusBadgeInTable(orderId, data.newStatus);
            
            // Hide modal
            bootstrap.Modal.getInstance(document.getElementById('statusUpdateModal')).hide();
            
            // Show success message
            showAlert('Order status updated successfully!', 'success');
            
            // If on details page, refresh the page to show updated timeline
            if (window.location.pathname.includes('/Details/')) {
                setTimeout(() => {
                    window.location.reload();
                }, 1000); // Reduced from 1500 to 1000 for faster feedback
            } else {
                // If on index page, just update the table row
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            }
        } else {
            showAlert(data.message || 'Failed to update order status', 'danger');
        }
    })
    .catch(error => {
        console.error('Error updating order status:', error);
        showAlert('An error occurred while updating the order status', 'danger');
    })
    .finally(() => {
        // Restore button state
        updateButton.innerHTML = originalText;
        updateButton.disabled = false;
    });
}

// Update status badge in the table after successful update
function updateStatusBadgeInTable(orderId, newStatus) {
    const row = document.querySelector(`tr[data-order-id="${orderId}"]`);
    if (row) {
        const statusCell = row.querySelector('.order-status');
        if (statusCell) {
            // Remove old badge classes
            statusCell.className = 'badge order-status';
            
            // Add new badge class and update content
            statusCell.classList.add(getStatusBadgeClass(newStatus));
            statusCell.innerHTML = `<i class="${getStatusIcon(newStatus)} me-1"></i>${newStatus}`;
        }
    }
}

// Get CSS class for status badge
function getStatusBadgeClass(status) {
    const statusLower = status.toLowerCase();
    switch (statusLower) {
        case 'pending': return 'bg-warning';
        case 'approved': return 'bg-info';
        case 'shipped': return 'bg-primary';
        case 'delivered': return 'bg-success';
        case 'cancelled': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

// Get icon class for status
function getStatusIcon(status) {
    const statusLower = status.toLowerCase();
    switch (statusLower) {
        case 'pending': return 'fas fa-clock';
        case 'approved': return 'fas fa-check';
        case 'shipped': return 'fas fa-shipping-fast';
        case 'delivered': return 'fas fa-check-circle';
        case 'cancelled': return 'fas fa-times-circle';
        default: return 'fas fa-question-circle';
    }
}

// Copy tracking number to clipboard
function copyTrackingNumber(trackingNumber) {
    navigator.clipboard.writeText(trackingNumber).then(function() {
        showAlert(`Tracking number ${trackingNumber} copied to clipboard!`, 'success');
    }).catch(function(err) {
        console.error('Could not copy text: ', err);
        showAlert('Failed to copy tracking number', 'warning');
    });
}

// Refresh orders page
function refreshOrders() {
    showLoadingIndicator();
    window.location.reload();
}

// Export orders (placeholder function)
function exportOrders() {
    showAlert('Export functionality will be implemented soon', 'info');
}

// Show alert message
function showAlert(message, type = 'info') {
    // Remove existing alerts
    const existingAlert = document.querySelector('.alert-custom');
    if (existingAlert) {
        existingAlert.remove();
    }
    
    // Create new alert
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show alert-custom" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <i class="fas fa-${getAlertIcon(type)} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    // Insert into DOM
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        const alert = document.querySelector('.alert-custom');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}

// Get icon for alert type
function getAlertIcon(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'danger': return 'exclamation-triangle';
        case 'warning': return 'exclamation-triangle';
        case 'info': return 'info-circle';
        default: return 'info-circle';
    }
}

// Show loading indicator
function showLoadingIndicator() {
    // Create loading overlay
    const loadingHtml = `
        <div id="loadingOverlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; display: flex; align-items: center; justify-content: center;">
            <div class="text-center text-white">
                <div class="spinner-border mb-3" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div>Loading orders...</div>
            </div>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', loadingHtml);
}

// Hide loading indicator
function hideLoadingIndicator() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.remove();
    }
}

// Get anti-forgery token
function getAntiForgeryToken() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    return token ? token.value : '';
}

// Utility function to format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('ar-EG', {
        style: 'currency',
        currency: 'EGP'
    }).format(amount);
}

// Utility function to format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Handle window load to hide loading indicator
window.addEventListener('load', function() {
    hideLoadingIndicator();
});

// Export functions to global scope for inline event handlers
window.openStatusUpdateModal = openStatusUpdateModal;
window.updateOrderStatus = updateOrderStatus;
window.copyTrackingNumber = copyTrackingNumber;
window.refreshOrders = refreshOrders;
window.exportOrders = exportOrders;