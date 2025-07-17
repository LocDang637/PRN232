// src/services/authService.js
import {jwtDecode} from 'jwt-decode';

export const authService = {
  // Login and store token
  login(token, user) {
    localStorage.setItem('authToken', token);
    localStorage.setItem('currentUser', JSON.stringify(user));
  },

  // Logout and clear storage
  logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
  },

  // Get current token
  getToken() {
    return localStorage.getItem('authToken');
  },

  // Get current user
  getCurrentUser() {
    const user = localStorage.getItem('currentUser');
    return user ? JSON.parse(user) : null;
  },

  // Check if user is authenticated
  isAuthenticated() {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded = jwtDecode(token);
      // Check if token is expired (with 5 minute buffer)
      return decoded.exp * 1000 > Date.now() - 300000;
    } catch (error) {
      return false;
    }
  },

  // Get user role
  getUserRole() {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decoded = jwtDecode(token);
      return decoded.role;
    } catch (error) {
      return null;
    }
  },

  // Check if user has specific role
  hasRole(role) {
    return this.getUserRole() === role;
  },

  // Check if user is admin
  isAdmin() {
    return this.hasRole('1');
  }
};

export default authService;