// Load Navbar
fetch('./assets/html/navbar.html')
.then(response => response.text())
.then(html => {
  document.getElementById('navbar-container').innerHTML = html;
});

// Load Footer
fetch('./assets/html/footer.html')
.then(response => response.text())
.then(html => {
  document.getElementById('footer-container').innerHTML = html;
});