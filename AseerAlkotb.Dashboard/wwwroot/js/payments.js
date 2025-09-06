// Payments Management JavaScript
// Handles AJAX operations for payment status updates, bulk operations, and interactive features

// Global variables
let currentPaymentId = null;
let selectedPayments = [];
let bulkModeActive = false;

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
        $('#paymentsTable').DataTable({
            "pageLength": 25,
            "order": [[ 8, "desc" ]], // Sort by date column descending
            "columnDefs": [
                { "orderable": false, "targets": [0, 9] } // Disable sorting for checkbox and actions columns
            ],
            "language": {
                "search": "Search payments:",
                "lengthMenu": "Show _MENU_ payments per page",
                "info": "Showing _START_ to _END_ of _TOTAL_ payments",
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
function openStatusUpdateModal(paymentId, transactionId, currentStatus, customerName) {
    currentPaymentId = paymentId;
    
    // Populate modal fields
    document.getElementById('paymentId').value = paymentId;
    document.getElementById('paymentInfo').textContent = `Payment #${paymentId} - ${transactionId} (${customerName})`;
    document.getElementById('currentStatus').innerHTML = `<span class="badge ${getStatusBadgeClass(currentStatus)}">${currentStatus}</span>`;
    
    // Reset form fields
    document.getElementById('newStatus').value = '';
    document.getElementById('adminNotes').value = '';
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('statusUpdateModal'));
    modal.show();
}

// Update payment status via AJAX
function updatePaymentStatus() {
    const paymentId = document.getElementById('paymentId').value;
    const newStatus = document.getElementById('newStatus').value;
    const adminNotes = document.getElementById('adminNotes').value;
    
    if (!paymentId || !newStatus) {
        showAlert('Please select a new status', 'warning');
        return;
    }
    
    // Show loading state
    const updateButton = document.querySelector('#statusUpdateModal .btn-primary');
    const originalText = updateButton.innerHTML;
    updateButton.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Updating...';
    updateButton.disabled = true;
    
    // Prepare request data - match the UpdatePaymentStatusRequest structure
    const requestData = {
        PaymentId: parseInt(paymentId),
        NewStatus: parseInt(newStatus),
        Notes: adminNotes || null
    };
    
    // Make AJAX request
    fetch('/Payments/UpdateStatus', {
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
            updateStatusBadgeInTable(paymentId, data.newStatus);
            
            // Hide modal
            bootstrap.Modal.getInstance(document.getElementById('statusUpdateModal')).hide();
            
            // Show success message
            showAlert('Payment status updated successfully!', 'success');
            
            // Refresh the page to show updated data
            setTimeout(() => {
                window.location.reload();
            }, 1000);
        } else {
            showAlert(data.message || 'Failed to update payment status', 'danger');
        }
    })
    .catch(error => {
        console.error('Error updating payment status:', error);
        showAlert('An error occurred while updating the payment status', 'danger');
    })
    .finally(() => {
        // Restore button state
        updateButton.innerHTML = originalText;
        updateButton.disabled = false;
    });
}

// Toggle bulk mode
function toggleBulkMode() {
    bulkModeActive = !bulkModeActive;
    
    const checkboxCells = document.querySelectorAll('.bulk-checkbox');
    const bulkPanel = document.getElementById('bulkActionsPanel');
    
    if (bulkModeActive) {
        checkboxCells.forEach(cell => cell.style.display = 'table-cell');
        bulkPanel.style.display = 'block';
    } else {
        checkboxCells.forEach(cell => cell.style.display = 'none');
        bulkPanel.style.display = 'none';
        // Clear selections
        selectedPayments = [];
        document.querySelectorAll('.payment-checkbox').forEach(cb => cb.checked = false);
        document.getElementById('selectAll').checked = false;
        updateSelectedCount();
    }
}

// Toggle select all checkboxes
function toggleSelectAll() {
    const selectAllCheckbox = document.getElementById('selectAll');
    const paymentCheckboxes = document.querySelectorAll('.payment-checkbox');
    
    paymentCheckboxes.forEach(checkbox => {
        checkbox.checked = selectAllCheckbox.checked;
    });
    
    updateSelectedCount();
}

// Update selected count
function updateSelectedCount() {
    const checkedBoxes = document.querySelectorAll('.payment-checkbox:checked');
    selectedPayments = Array.from(checkedBoxes).map(cb => parseInt(cb.value));
    
    document.getElementById('selectedCount').textContent = selectedPayments.length;
    
    // Update select all checkbox state
    const allCheckboxes = document.querySelectorAll('.payment-checkbox');
    const selectAllCheckbox = document.getElementById('selectAll');
    
    if (selectedPayments.length === 0) {
        selectAllCheckbox.indeterminate = false;
        selectAllCheckbox.checked = false;
    } else if (selectedPayments.length === allCheckboxes.length) {
        selectAllCheckbox.indeterminate = false;
        selectAllCheckbox.checked = true;
    } else {
        selectAllCheckbox.indeterminate = true;
    }
}

// Bulk update status
function bulkUpdateStatus() {
    const newStatus = document.getElementById('bulkStatusSelect').value;
    
    if (selectedPayments.length === 0) {
        showAlert('Please select at least one payment', 'warning');
        return;
    }
    
    if (!newStatus) {
        showAlert('Please select a new status', 'warning');
        return;
    }
    
    // Confirm bulk action
    if (!confirm(`Are you sure you want to update ${selectedPayments.length} payment(s) status?`)) {
        return;
    }
    
    // Show loading state
    const bulkUpdateBtn = document.getElementById('bulkUpdateBtn');
    const originalText = bulkUpdateBtn.innerHTML;
    bulkUpdateBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Updating...';
    bulkUpdateBtn.disabled = true;
    
    // Make AJAX request
    fetch('/Payments/BulkUpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        body: JSON.stringify({
            PaymentIds: selectedPayments,
            NewStatus: parseInt(newStatus)
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showAlert(data.message, 'success');
            setTimeout(() => {
                window.location.reload();
            }, 1500);
        } else {
            showAlert(data.message || 'Failed to update payments', 'danger');
        }
    })
    .catch(error => {
        console.error('Error in bulk update:', error);
        showAlert('An error occurred during bulk update', 'danger');
    })
    .finally(() => {
        // Restore button state
        bulkUpdateBtn.innerHTML = originalText;
        bulkUpdateBtn.disabled = false;
    });
}

// Clear all selections
function clearAllSelections() {
    selectedPayments = [];
    document.querySelectorAll('.payment-checkbox').forEach(cb => cb.checked = false);
    document.getElementById('selectAll').checked = false;
    updateSelectedCount();
}

// Update status badge in the table after successful update
function updateStatusBadgeInTable(paymentId, newStatus) {
    const row = document.querySelector(`tr[data-payment-id="${paymentId}"]`);
    if (row) {
        const statusCell = row.querySelector('.payment-status');
        if (statusCell) {
            // Remove old badge classes
            statusCell.className = 'badge payment-status';
            
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
        case 'processing': return 'bg-info';
        case 'paid': return 'bg-success';
        case 'failed': return 'bg-danger';
        case 'cancelled': return 'bg-secondary';
        case 'refunded': return 'bg-dark';
        case 'partiallyrefunded': return 'bg-primary';
        default: return 'bg-secondary';
    }
}

// Get icon class for status
function getStatusIcon(status) {
    const statusLower = status.toLowerCase();
    switch (statusLower) {
        case 'pending': return 'fas fa-clock';
        case 'processing': return 'fas fa-spinner fa-spin';
        case 'paid': return 'fas fa-check-circle';
        case 'failed': return 'fas fa-times-circle';
        case 'cancelled': return 'fas fa-ban';
        case 'refunded': return 'fas fa-undo';
        case 'partiallyrefunded': return 'fas fa-undo-alt';
        default: return 'fas fa-question-circle';
    }
}

// Copy text to clipboard
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function() {
        showAlert(`${text} copied to clipboard!`, 'success');
    }).catch(function(err) {
        console.error('Could not copy text: ', err);
        showAlert('Failed to copy to clipboard', 'warning');
    });
}

// View Paymob details (placeholder)
function viewPaymobDetails(paymobOrderId) {
    if (paymobOrderId) {
        // Open Paymob dashboard in new tab
        window.open(`https://accept.paymob.com/portal/en/orders/${paymobOrderId}`, '_blank');
    } else {
        showAlert('Paymob order ID not available', 'warning');
    }
}

// Initiate refund (placeholder)
function initiateRefund(paymentId) {
    if (confirm('Are you sure you want to initiate a refund for this payment?')) {
        showAlert('Refund functionality will be implemented soon', 'info');
    }
}

// Refresh payments page
function refreshPayments() {
    showLoadingIndicator();
    window.location.reload();
}

// Export payments (placeholder function)
function exportPayments() {
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
                <div>Loading payments...</div>
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
window.updatePaymentStatus = updatePaymentStatus;
window.toggleBulkMode = toggleBulkMode;
window.toggleSelectAll = toggleSelectAll;
window.updateSelectedCount = updateSelectedCount;
window.bulkUpdateStatus = bulkUpdateStatus;
window.clearAllSelections = clearAllSelections;
window.copyToClipboard = copyToClipboard;
window.viewPaymobDetails = viewPaymobDetails;
window.initiateRefund = initiateRefund;
window.refreshPayments = refreshPayments;
window.exportPayments = exportPayments;