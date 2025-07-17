import React from 'react';
import Navbar from './Navbar';
import { useAuth } from '../../context/AuthContext';

const Layout = ({ children }) => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-vh-100 bg-light">
      {isAuthenticated() && <Navbar />}
      <main className={isAuthenticated() ? 'py-4' : ''}>
        {children}
      </main>
    </div>
  );
};

export default Layout;