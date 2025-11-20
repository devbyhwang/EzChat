// EzChat 클라이언트 스크립트
document.addEventListener('DOMContentLoaded', function() {
    // 알림 자동 숨김
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(function(alert) {
        setTimeout(function() {
            alert.style.opacity = '0';
            setTimeout(function() {
                alert.remove();
            }, 300);
        }, 5000);
    });
});
