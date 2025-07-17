// src/components/Layout/Navbar.js
import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const Navbar = () => {
  const { user, logout, isAdmin } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path) => {
    return location.pathname === path ? 'active' : '';
  };

  const getUserRole = () => {
    if (!user) return '';
    switch (user.roleId.toString()) {
      case '1': return 'Admin';
      case '2': return 'Manager';
      case '3': return 'User';
      default: return 'User';
    }
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-primary shadow">
      <div className="container">
        <Link className="navbar-brand fw-bold" to="/chats">
          <i className="bi bi-shield-check me-2"></i>
          SmokeQuit
        </Link>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link 
                className={`nav-link ${isActive('/chats')}`} 
                to="/chats"
              >
                <i className="bi bi-chat-dots me-1"></i>
                Chats
              </Link>
            </li>
            <li className="nav-item">
              <Link 
                className={`nav-link ${isActive('/coaches')}`} 
                to="/coaches"
              >
                <i className="bi bi-people me-1"></i>
                Coaches
              </Link>
            </li>
          </ul>

          <ul className="navbar-nav">
            <li className="nav-item dropdown">
              <a
                className="nav-link dropdown-toggle d-flex align-items-center"
                href="#"
                id="navbarDropdown"
                role="button"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                <div className="d-flex align-items-center">
                  <div className="me-2">
                    <div className="rounded-circle bg-light text-primary d-flex align-items-center justify-content-center" 
                         style={{ width: '32px', height: '32px' }}>
                      <i className="bi bi-person-fill"></i>
                    </div>
                  </div>
                  <div className="d-none d-md-block">
                    <div className="fw-semibold">{user?.fullName}</div>
                    <small className="text-light opacity-75">{getUserRole()}</small>
                  </div>
                </div>
              </a>
              <ul className="dropdown-menu dropdown-menu-end">
                <li>
                  <div className="dropdown-header">
                    <div className="fw-bold">{user?.fullName}</div>
                    <small className="text-muted">{user?.email}</small>
                  </div>
                </li>
                <li><hr className="dropdown-divider" /></li>
                <li>
                  <span className="dropdown-item-text">
                    <i className="bi bi-shield-check text-success me-2"></i>
                    <small>Role: {getUserRole()}</small>
                  </span>
                </li>
                <li>
                  <span className="dropdown-item-text">
                    <i className="bi bi-person-badge text-info me-2"></i>
                    <small>ID: {user?.employeeCode}</small>
                  </span>
                </li>
                <li><hr className="dropdown-divider" /></li>
                <li>
                  <button 
                    className="dropdown-item text-danger" 
                    onClick={handleLogout}
                  >
                    <i className="bi bi-box-arrow-right me-2"></i>
                    Logout
                  </button>
                </li>
              </ul>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;