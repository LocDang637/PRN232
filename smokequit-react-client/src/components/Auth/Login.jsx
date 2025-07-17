import React, { useState } from 'react';
import { useMutation } from '@apollo/client';
import { useAuth } from '../../context/AuthContext';
import { LOGIN_MUTATION } from '../../services/graphqlQueries';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [formData, setFormData] = useState({
    username: '',
    password: ''
  });
  const [errors, setErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const [loginMutation, { loading }] = useMutation(LOGIN_MUTATION, {
    onCompleted: (data) => {
      if (data.login && data.login.token) {
        login(data.login.token, data.login.user);
        navigate('/chats');
      }
    },
    onError: (error) => {
      console.error('Login error:', error);
      setErrors({ 
        general: error.graphQLErrors?.[0]?.message || 'Login failed. Please try again.' 
      });
    }
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    }
    
    if (!formData.password) {
      newErrors.password = 'Password is required';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) return;
    
    try {
      await loginMutation({
        variables: {
          username: formData.username,
          password: formData.password
        }
      });
    } catch (error) {
      // Error handled in onError callback
    }
  };

  return (
    <div className="container-fluid vh-100 d-flex align-items-center justify-content-center bg-light">
      <div className="row w-100">
        <div className="col-md-6 col-lg-4 mx-auto">
          <div className="card shadow">
            <div className="card-body p-4">
              <div className="text-center mb-4">
                <h2 className="card-title text-primary">
                  <i className="bi bi-shield-check me-2"></i>
                  SmokeQuit
                </h2>
                <p className="text-muted">Sign in to your account</p>
              </div>

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="username" className="form-label">
                    Username
                  </label>
                  <input
                    type="text"
                    className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                    id="username"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                    placeholder="Enter username"
                    disabled={loading}
                  />
                  {errors.username && (
                    <div className="invalid-feedback">{errors.username}</div>
                  )}
                </div>

                <div className="mb-3">
                  <label htmlFor="password" className="form-label">
                    Password
                  </label>
                  <div className="input-group">
                    <input
                      type={showPassword ? 'text' : 'password'}
                      className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                      id="password"
                      name="password"
                      value={formData.password}
                      onChange={handleChange}
                      placeholder="Enter password"
                      disabled={loading}
                    />
                    <button
                      type="button"
                      className="btn btn-outline-secondary"
                      onClick={() => setShowPassword(!showPassword)}
                      disabled={loading}
                    >
                      <i className={`bi ${showPassword ? 'bi-eye-slash' : 'bi-eye'}`}></i>
                    </button>
                    {errors.password && (
                      <div className="invalid-feedback">{errors.password}</div>
                    )}
                  </div>
                </div>

                {errors.general && (
                  <div className="alert alert-danger" role="alert">
                    <i className="bi bi-exclamation-triangle-fill me-2"></i>
                    {errors.general}
                  </div>
                )}

                <div className="d-grid mb-3">
                  <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={loading}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Signing in...
                      </>
                    ) : (
                      <>
                        <i className="bi bi-box-arrow-in-right me-2"></i>
                        Sign In
                      </>
                    )}
                  </button>
                </div>
              </form>

              <div className="text-center">
                <small className="text-muted">
                  Secured with JWT Authentication
                  <i className="bi bi-shield-check text-success ms-1"></i>
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;