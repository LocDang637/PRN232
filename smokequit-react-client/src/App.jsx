import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ApolloProvider } from '@apollo/client';
import apolloClient from './services/apolloClient';
import { AuthProvider } from './context/AuthContext';
import Layout from './components/Layout/Layout';
import ProtectedRoute from './components/Auth/ProtectedRoute';
import Login from './components/Auth/Login';
import ChatList from './components/Chat/ChatList';
import CoachList from './components/Coach/CoachList';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-icons/font/bootstrap-icons.css';

function App() {
  return (
    <ApolloProvider client={apolloClient}>
      <AuthProvider>
        <Router>
          <Layout>
            <Routes>
              {/* Public Routes */}
              <Route path="/login" element={<Login />} />
              
              {/* Protected Routes */}
              <Route 
                path="/chats" 
                element={
                  <ProtectedRoute>
                    <ChatList />
                  </ProtectedRoute>
                } 
              />
              
              <Route 
                path="/coaches" 
                element={
                  <ProtectedRoute>
                    <CoachList />
                  </ProtectedRoute>
                } 
              />
              
              {/* Default redirect */}
              <Route path="/" element={<Navigate to="/chats" replace />} />
              
              {/* Catch all - redirect to chats */}
              <Route path="*" element={<Navigate to="/chats" replace />} />
            </Routes>
          </Layout>
        </Router>
      </AuthProvider>
    </ApolloProvider>
  );
}

export default App;
