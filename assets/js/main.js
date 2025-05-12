/**
 * Portfolio website functionality
 */

document.addEventListener('DOMContentLoaded', () => {
    // Mobile menu toggle
    const hamburgerMenu = document.getElementById('hamburger-menu');
    const mobileMenu = document.getElementById('mobile-menu');
    
    if (hamburgerMenu && mobileMenu) {
      hamburgerMenu.addEventListener('click', toggleMobileMenu);
    }
    
    /**
     * Toggles the mobile menu visibility
     */
    function toggleMobileMenu() {
      mobileMenu.classList.toggle('show-menu');
      
      // Update ARIA attributes for accessibility
      const isExpanded = mobileMenu.classList.contains('show-menu');
      hamburgerMenu.setAttribute('aria-expanded', isExpanded);
      
      // Add focus trap for keyboard navigation when menu is open
      if (isExpanded) {
        trapFocus(mobileMenu);
      }
    }
    
    /**
     * Trap focus within an element for better accessibility
     * @param {HTMLElement} element - The element to trap focus within
     */
    function trapFocus(element) {
      const focusableElements = element.querySelectorAll(
        'a[href], button, input, textarea, select, [tabindex]:not([tabindex="-1"])'
      );
      
      if (focusableElements.length === 0) return;
      
      const firstElement = focusableElements[0];
      const lastElement = focusableElements[focusableElements.length - 1];
      
      // Focus the first element
      firstElement.focus();
      
      element.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
          toggleMobileMenu();
        }
        
        if (e.key === 'Tab') {
          // Shift + Tab pressed on first element should loop to last element
          if (e.shiftKey && document.activeElement === firstElement) {
            e.preventDefault();
            lastElement.focus();
          }
          // Tab pressed on last element should loop to first element
          else if (!e.shiftKey && document.activeElement === lastElement) {
            e.preventDefault();
            firstElement.focus();
          }
        }
      });
    }
    
    // Close mobile menu when clicking outside
    document.addEventListener('click', (e) => {
      if (mobileMenu && 
          mobileMenu.classList.contains('show-menu') && 
          !mobileMenu.contains(e.target) && 
          !hamburgerMenu.contains(e.target)) {
        toggleMobileMenu();
      }
    });
    
    // Handle window resize to ensure proper menu display
    window.addEventListener('resize', () => {
      if (window.innerWidth >= 768 && mobileMenu && mobileMenu.classList.contains('show-menu')) {
        mobileMenu.classList.remove('show-menu');
        hamburgerMenu.setAttribute('aria-expanded', 'false');
      }
    });
  });