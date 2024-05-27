<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Ubiquiti Device Discovery Tool</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/tailwindcss/2.2.19/tailwind.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/css/bootstrap.min.css" rel="stylesheet">
 
    <style>
        .progress-bar {
            line-height: 1.5rem;
        }

        .modal-content {
            max-width: 600px;
            margin: auto;
            border-radius: 0.75rem;
        }

        .highlight {
            background-color: #ffcccc;
        }

        .btn {
            display: flex;
            align-items: center;
            border-radius: 0.375rem;
            transition: all 0.3s ease;
        }

            .btn i {
                margin-right: 8px;
            }

        .table-auto tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table-auto tr:hover {
            background-color: #f1f1f1;
        }

        .header-gradient {
            background: linear-gradient(45deg, #007AFF, #00A1FF);
        }

        .progress-gradient {
            background: linear-gradient(45deg, #00c6ff, #0072ff);
        }

        .btn-primary {
            background-color: #007AFF;
            border-color: #007AFF;
        }

            .btn-primary:hover {
                background-color: #005BB5;
                border-color: #005BB5;
            }

        .btn-secondary {
            background-color: #6c757d;
            border-color: #6c757d;
        }

            .btn-secondary:hover {
                background-color: #545b62;
                border-color: #545b62;
            }

        .btn-success {
            background-color: #28a745;
            border-color: #28a745;
        }

            .btn-success:hover {
                background-color: #1e7e34;
                border-color: #1e7e34;
            }

        .btn-info {
            background-color: #17a2b8;
            border-color: #17a2b8;
        }

            .btn-info:hover {
                background-color: #117a8b;
                border-color: #117a8b;
            }

        .btn-danger {
            background-color: #dc3545;
            border-color: #dc3545;
        }

            .btn-danger:hover {
                background-color: #bd2130;
                border-color: #bd2130;
            }

        .btn-scroll {
            position: fixed;
            bottom: 20px;
            left: 20px;
            border-radius: 50%;
            background-color: #007AFF;
            width: 50px;
            height: 50px;
            display: flex;
            justify-content: center;
            align-items: center;
            color: white;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
            transition: background-color 0.3s ease, opacity 0.3s ease;
        }

            .btn-scroll:hover {
                background-color: #005BB5;
            }

            .btn-scroll.hidden {
                opacity: 0;
                pointer-events: none;
            }

        .connection-status {
            display: flex;
            justify-content: center;
            align-items: center;
        }

            .connection-status span {
                width: 15px;
                height: 15px;
                border-radius: 50%;
                display: inline-block;
                background-color: currentColor;
                margin: 0 auto; /* لتوسيط النقطة */
            }

        .signal-strong {
            background-color: #28a745; /* لون أخضر */
            color: white;
        }

        .signal-medium {
            background-color: #ffc107; /* لون أصفر */
            color: black;
        }

        .signal-weak {
            background-color: #dc3545; /* لون أحمر */
            color: white;
        }


        body {
            padding-top: 70px; /* Adjusted for the fixed navbar */
        }

        .navbar-custom {
            background-color: #343a40; /* Custom color */
        }

        .navbar-brand {
            font-size: 1.5rem;
        }

        .navbar-nav .nav-item {
            margin-left: 1rem;
        }

            .navbar-nav .nav-item .btn {
                margin-left: 1rem;
            }
    </style>
</head>
<body class="bg-gray-100 p-6">
    <nav class="navbar navbar-expand-lg navbar-dark navbar-custom fixed-top">
        <div class="container-fluid">
            <a class="navbar-brand" href="#">Ubiquiti Device Discovery Tool</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item">
                        <button class="btn btn-primary text-white shadow" onclick="showLoginModal()"><i class="fas fa-sign-in-alt"></i> Login</button>
                    </li>
                    <li class="nav-item">
                        <button class="btn btn-secondary text-white shadow" onclick="showRegisterModal()"><i class="fas fa-user-plus"></i> Sign Up</button>
                    </li>
                </ul>
            </div>
        </div>
    </nav>

    <div class="container mx-auto animate__animated animate__fadeIn">
        <div class="flex items-center justify-between mb-6 mt-5">
            <div class="flex items-center space-x-2">
                <input type="text" class="form-control p-2 rounded border border-gray-300 shadow" placeholder="IP, Name, MAC" id="searchInput" oninput="filterTable()">
                <button class="btn btn-primary p-2 text-white shadow" onclick="searchDevices()"><i class="fas fa-search"></i> Search</button>
            </div>
            <div class="flex items-center space-x-4">
                <button class="btn btn-secondary p-2 text-white shadow" onclick="clearDevices()"><i class="fas fa-trash-alt"></i> Clean</button>
                <button class="btn btn-info p-2 text-white shadow" onclick="refreshDevices()"><i class="fas fa-sync-alt"></i> Refresh</button>
                <button class="btn btn-success p-2 text-white shadow" onclick="updateDevices()"><i class="fas fa-upload"></i> Up</button>
                <button class="btn btn-primary p-2 text-white shadow" data-bs-toggle="modal" data-bs-target="#settingsModal"><i class="fas fa-cog"></i> Settings</button>
                <button class="btn btn-danger p-2 text-white shadow" onclick="exportToPDF()"><i class="fas fa-file-pdf"></i> Export</button>
                <div class="ml-4 text-lg font-medium">Devices: <span id="deviceCount">0</span></div>
            </div>
        </div>
        <div class="w-full progress-gradient h-8 rounded mb-4 shadow">
            <div id="progressBar" class="bg-green-700 h-full text-center text-white progress-bar" style="width: 0%;">0%</div>
        </div>
        <div class="overflow-x-auto bg-white rounded-lg shadow-md">
                <table class="table-auto w-full text-left rounded-lg" id="deviceTable">
                    <thead class="bg-gray-200">
                        <tr>
                            <th class="px-4 py-2">Status</th>
                            <th class="px-4 py-2">Hardware Address</th>
                            <th class="px-4 py-2">IP Address</th>
                            <th class="px-4 py-2">Name</th>
                            <th class="px-4 py-2 sortable" onclick="sortTableBySignal()">Signal</th>
                            <th class="px-4 py-2">UP TIME</th>
                            <th class="px-4 py-2">SSID</th>
                            <th class="px-4 py-2">Channel</th>
                            <th class="px-4 py-2">Firmware Version</th>
                            <th class="px-4 py-2">Open</th>
                        </tr>
                    </thead>
                    <tbody id="deviceTableBody">
                        <tr>
                            <td class="px-4 py-2 connection-status"><span></span></td>
                            <td class="px-4 py-2">24:A4:3C:D2:C2:2F</td>
                            <td class="px-4 py-2">10.137.205.223</td>
                            <td class="px-4 py-2">NanoStation M5 'Ahmed abas'</td>
                            <td class="px-4 py-2">N/A</td>
                            <td class="px-4 py-2">N/A</td>
                            <td class="px-4 py-2">N/A</td>
                            <td class="px-4 py-2">N/A</td>
                            <td class="px-4 py-2">N/A</td>
                            <td class="px-4 py-2"><a href="#" class="text-blue-500 hover:underline">Open</a></td>
                        </tr>
                        <!-- Additional rows can be added here -->
                    </tbody>
                </table>
        </div>
    </div>

    <!-- Scroll to Bottom Button -->
    <button id="scrollButton" class="btn-scroll" onclick="scrollToBottom()"><i class="fas fa-arrow-down"></i></button>

    <!-- Settings Modal -->
    <div class="modal fade" id="settingsModal" tabindex="-1" aria-labelledby="settingsModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content p-6">
                <div class="modal-header">
                    <h5 class="modal-title" id="settingsModalLabel">Settings</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form id="settingsForm">
                        <div class="form-group mb-4">
                            <label for="ipAddress" class="block text-gray-700">IP Address</label>
                            <input type="text" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="ipAddress" placeholder="Enter IP address">
                        </div>
                        <div class="form-group mb-4">
                            <label for="username" class="block text-gray-700">Username</label>
                            <input type="text" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="username" placeholder="Enter username">
                        </div>
                        <div class="form-group mb-4">
                            <label for="password" class="block text-gray-700">Password</label>
                            <input type="password" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="password" placeholder="Enter password">
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-success p-2 rounded-lg bg-green-500 text-white hover:bg-green-700 transition shadow" onclick="saveSettings()">Save</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Login Modal -->
    <div class="modal fade" id="loginModal" tabindex="-1" aria-labelledby="loginModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content p-6">
                <div class="modal-header">
                    <h5 class="modal-title" id="loginModalLabel">Login</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form id="loginForm">
                        <div class="form-group mb-4">
                            <label for="loginUsername" class="block text-gray-700">Username</label>
                            <input type="text" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="loginUsername" placeholder="Enter username">
                        </div>
                        <div class="form-group mb-4">
                            <label for="loginPassword" class="block text-gray-700">Password</label>
                            <input type="password" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="loginPassword" placeholder="Enter password">
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary p-2 rounded-lg bg-blue-500 text-white hover:bg-blue-700 transition shadow" onclick="login()">Login</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Register Modal -->
    <div class="modal fade" id="registerModal" tabindex="-1" aria-labelledby="registerModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content p-6">
                <div class="modal-header">
                    <h5 class="modal-title" id="registerModalLabel">Sign Up</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form id="registerForm">
                        <div class="form-group mb-4">
                            <label for="registerUsername" class="block text-gray-700">Username</label>
                            <input type="text" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="registerUsername" placeholder="Enter username">
                        </div>
                        <div class="form-group mb-4">
                            <label for="registerEmail" class="block text-gray-700">Email</label>
                            <input type="email" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="registerEmail" placeholder="Enter email">
                        </div>
                        <div class="form-group mb-4">
                            <label for="registerPassword" class="block text-gray-700">Password</label>
                            <input type="password" class="form-control mt-2 p-2 rounded border border-gray-300 w-full" id="registerPassword" placeholder="Enter password">
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary p-2 rounded-lg bg-gray-500 text-white hover:bg-gray-700 transition shadow" onclick="register()">Sign Up</button>
                </div>
            </div>
        </div>
    </div>
   
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.3.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.4.0/jspdf.umd.min.js"></script>
    <!-- ثم تحميل jsPDF autotable -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.13/jspdf.plugin.autotable.min.js"></script>



    <script>
        function scrollToBottom() {
            window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
            toggleScrollButton(true);
        }

        function scrollToTop() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
            toggleScrollButton(false);
        }

        function toggleScrollButton(isBottom) {
            const scrollButton = document.getElementById('scrollButton');
            if (isBottom || window.scrollY + window.innerHeight >= document.body.scrollHeight) {
                scrollButton.innerHTML = '<i class="fas fa-arrow-up"></i>';
                scrollButton.onclick = scrollToTop;
            } else {
                scrollButton.innerHTML = '<i class="fas fa-arrow-down"></i>';
                scrollButton.onclick = scrollToBottom;
            }
        }

        function getBaseRange(ipAddress) {
            const parts = ipAddress.split('.');
            return parts.length === 4 ? parts.slice(0, 3).join('.') + '.' : '';
        }

        function updateDeviceTable(devices) {
            const tableBody = document.getElementById('deviceTableBody');
            tableBody.innerHTML = '';
            let progressStep = 100 / devices.length;
            let currentProgress = 0;

            const ipAddress = document.getElementById('ipAddress').value;
            const baseRange = getBaseRange(ipAddress);

            devices.forEach((device, index) => {
                setTimeout(() => {
                    const row = document.createElement('tr');
                    const deviceIP = device.Host;
                    const isDifferentRange = !deviceIP.startsWith(baseRange);

                    row.innerHTML = `
                                                        <td class="px-4 py-2 connection-status"><span></span></td>
                                                        <td class="border px-4 py-2">${device.MAC}</td>
                                                        <td class="border px-4 py-2">${deviceIP}</td>
                                                        <td class="border px-4 py-2">${device.Name}</td>
                                                        <td class="border px-4 py-2">${device.Signal || 'N/A'}</td>
                                                        <td class="border px-4 py-2">${device.Uptime || 'N/A'}</td>
                                                        <td class="border px-4 py-2">${device.SSID || 'N/A'}</td>
                                                        <td class="border px-4 py-2">${device.Channel || 'N/A'}</td>
                                                        <td class="border px-4 py-2">${device.Firmware || 'N/A'}</td>
                                                        <td class="border px-4 py-2"><a href="http://${device.Host}" target="_blank" class="text-blue-500 hover:underline">Open</a></td>
                                                    `;

                    if (isDifferentRange) row.classList.add('highlight');
                    tableBody.appendChild(row);
                    currentProgress += progressStep;
                    updateProgressBar(Math.min(currentProgress, 100));
                }, index * 50);
            });

            document.getElementById('deviceCount').innerText = devices.length;
            updateProgressBar(100);
        }
        function updateProgressBar(progress) {
            const progressBar = document.getElementById('progressBar');
            progressBar.style.width = progress + '%';
            progressBar.innerText = Math.round(progress) + '%';
        }

        function saveSettings() {
            const ipAddress = document.getElementById('ipAddress').value;
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;

            sessionStorage.setItem('username', username);
            sessionStorage.setItem('password', password);

            $.ajax({
                type: 'POST',
                url: '/Devices/SaveSettings',
                data: { ipAddress, username, password },
                beforeSend: () => updateProgressBar(0),
                success: response => {
                    if (response.success) {
                        alert('Settings saved successfully.');
                        $('#settingsModal').modal('hide');
                        updateDeviceTable(response.devices);
                    } else alert('Error saving settings: ' + response.message);
                },
                error: () => alert('Error saving settings.')
            });
        }

        function searchDevices() { /* Add search functionality */ }
        function clearDevices() {
            document.getElementById('deviceTableBody').innerHTML = '';
            document.getElementById('deviceCount').innerText = 0;
            updateProgressBar(0);
        }

        function refreshDevices() {
            connectToAllDevices();
        }

       function exportToPDF() {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF();

            // استخراج البيانات فقط للأعمدة المطلوبة
            const table = document.getElementById("deviceTable");
            const rows = table.querySelectorAll("tr");
            const data = [];
            rows.forEach(row => {
                const cells = row.querySelectorAll("th, td");
                data.push([
                    cells[3] ? cells[3].innerText : "", // Name
                    cells[2] ? cells[2].innerText : "", // IP Address
                    cells[4] ? cells[4].innerText : "", // Signal
                    cells[5] ? cells[5].innerText : ""  // UP TIME
                ]);
            });

            // إنشاء جدول PDF
            doc.autoTable({
                head: [['Name', 'IP Address', 'Signal', 'UP TIME']],
                body: data.slice(1) // استبعاد الرأس
            });

            doc.save('table.pdf');
        }

        function filterTable() {
            const searchInput = document.getElementById('searchInput').value.toLowerCase();
            const rows = document.querySelectorAll('#deviceTableBody tr');
            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                let match = Array.from(cells).some(cell => cell.textContent.toLowerCase().includes(searchInput));
                row.style.display = match ? '' : 'none';
            });
        }

        document.addEventListener('DOMContentLoaded', () => {
            toggleScrollButton(false);
        });

        window.addEventListener('scroll', () => {
            if (window.scrollY + window.innerHeight >= document.body.scrollHeight) {
                toggleScrollButton(true);
            } else {
                toggleScrollButton(false);
            }
        });

        function sortTableBySignal() {
            const table = document.getElementById('deviceTableBody');
            const rows = Array.from(table.rows);

            rows.sort((a, b) => {
                const signalA = parseInt(a.cells[4].textContent);
                const signalB = parseInt(b.cells[4].textContent);
                return signalA - signalB;
            });

            rows.forEach(row => table.appendChild(row));
        }

        // Add event listener to the Signal header
        document.querySelector('th.sortable').addEventListener('click', sortTableBySignal);

        function connectToDevice(ip) {
            const username = sessionStorage.getItem('username');
            const password = sessionStorage.getItem('password');

            return $.ajax({
                type: 'POST',
                url: '/Devices/ConnectToDevice',
                data: JSON.stringify({ ip, username, password }),
                contentType: 'application/json'
            });
        }

        function getBaseRange(ipAddress) {
            const parts = ipAddress.split('.');
            return parts.length === 4 ? parts.slice(0, 3).join('.') + '.' : '';
        }

        function colorSignalCell(cell, signal) {
            cell.classList.remove('signal-strong', 'signal-medium', 'signal-weak');
            if (signal < -60) {
                cell.classList.add('signal-weak'); // Red
            } else if (signal < -50) {
                cell.classList.add('signal-medium'); // Yellow
            } else {
                cell.classList.add('signal-strong'); // Green
            }
        }

        // Modify the existing function to include the coloring
        function connectToAllDevices() {
            const rows = document.querySelectorAll('#deviceTableBody tr');
            const promises = [];
            let progressStep = 100 / rows.length;
            let currentProgress = 0;

            const username = sessionStorage.getItem('username');
            const password = sessionStorage.getItem('password');

            rows.forEach(row => {
                const ip = row.cells[2].textContent;

                const promise = fetch('/Devices/ConnectToDevice', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ ip, username, password })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            const deviceInfo = data.deviceInfo;
                            row.cells[0].querySelector('span').style.backgroundColor = '#28a745'; // لون أخضر مميز
                            row.cells[4].textContent = deviceInfo.signal;
                            row.cells[5].textContent = deviceInfo.uptime;
                            row.cells[6].textContent = deviceInfo.ssid;
                            row.cells[7].textContent = deviceInfo.frequency;
                            row.cells[8].textContent = deviceInfo.firmwareVersion;

                            // إضافة تلوين الإشارة
                            const signalValue = parseInt(deviceInfo.signal);
                            const signalCell = row.cells[4];
                            colorSignalCell(signalCell, signalValue);
                        } else {
                            row.cells[0].querySelector('span').style.backgroundColor = '#dc3545'; // لون أحمر مميز
                        }
                        currentProgress += progressStep;
                        updateProgressBar(Math.min(currentProgress, 100));
                    })
                    .catch(() => {
                        row.cells[0].querySelector('span').style.backgroundColor = '#dc3545'; // لون أحمر مميز في حالة الخطأ
                        currentProgress += progressStep;
                        updateProgressBar(Math.min(currentProgress, 100));
                    });

                promises.push(promise);
            });

            Promise.allSettled(promises).then(() => {
                console.log('All devices processed');
            });
        }

        // Add this function to sort the table by signal
        function sortTableBySignal() {
            const table = document.getElementById('deviceTableBody');
            const rows = Array.from(table.rows);

            rows.sort((a, b) => {
                const signalA = parseInt(a.cells[4].textContent);
                const signalB = parseInt(b.cells[4].textContent);
                return signalB - signalA;
            });

            rows.forEach(row => table.appendChild(row));
        }

        // Add event listener to the Signal header (this can be skipped since we already added the onclick in HTML)
        document.querySelector('th.sortable').addEventListener('click', sortTableBySignal);

        function toggleScrollButton(isBottom) {
            const scrollButton = document.getElementById('scrollButton');
            if (isBottom || window.scrollY + window.innerHeight >= document.body.scrollHeight) {
                scrollButton.innerHTML = '<i class="fas fa-arrow-up"></i>';
                scrollButton.onclick = scrollToTop;
            } else {
                scrollButton.innerHTML = '<i class="fas fa-arrow-down"></i>';
                scrollButton.onclick = scrollToBottom;
            }
        }

        function scrollToBottom() {
            window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
            toggleScrollButton(true);
        }

        function scrollToTop() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
            toggleScrollButton(false);
        }


        function showLoginModal() {
            $('#loginModal').modal('show');
        }

        function login() {
            const username = document.getElementById('loginUsername').value;
            const password = document.getElementById('loginPassword').value;

            // تنفيذ عملية تسجيل الدخول هنا (مثلاً، إرسال البيانات إلى الخادم)
            alert(`Logging in with Username: ${username} and Password: ${password}`);
            $('#loginModal').modal('hide');
        }

        function showRegisterModal() {
            $('#registerModal').modal('show');
        }

        function register() {
            const username = document.getElementById('registerUsername').value;
            const email = document.getElementById('registerEmail').value;
            const password = document.getElementById('registerPassword').value;

            // تنفيذ عملية إنشاء الحساب هنا (مثلاً، إرسال البيانات إلى الخادم)
            alert(`Registering with Username: ${username}, Email: ${email}, and Password: ${password}`);
            $('#registerModal').modal('hide');
        }
    </script>
</body>
</html>
