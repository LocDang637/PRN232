import React, { useState, useEffect } from 'react';
import { useMutation } from '@apollo/client';
import { CREATE_COACH, UPDATE_COACH } from '../../services/graphqlQueries';

const CoachForm = ({ coach, onClose }) => {
  const isEditing = !!coach;

  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phoneNumber: '',
    bio: ''
  });

  const [errors, setErrors] = useState({});

  const [createCoach, { loading: createLoading }] = useMutation(CREATE_COACH, {
    onCompleted: () => {
      onClose();
    },
    onError: (error) => {
      console.error('Create error:', error);
      setErrors({ general: error.message || 'Failed to create coach' });
    }
  });

  const [updateCoach, { loading: updateLoading }] = useMutation(UPDATE_COACH, {
    onCompleted: () => {
      onClose();
    },
    onError: (error) => {
      console.error('Update error:', error);
      setErrors({ general: error.message || 'Failed to update coach' });
    }
  });

  const loading = createLoading || updateLoading;

  useEffect(() => {
    if (isEditing && coach) {
      setFormData({
        fullName: coach.fullName || '',
        email: coach.email || '',
        phoneNumber: coach.phoneNumber || '',
        bio: coach.bio || ''
      });
    }
  }, [isEditing, coach]);

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

    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Full name is required';
    } else if (formData.fullName.trim().length < 2) {
      newErrors.fullName = 'Full name must be at least 2 characters';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email address';
    }

    if (formData.phoneNumber && !/^[\d\s\-\+\(\)]+$/.test(formData.phoneNumber)) {
      newErrors.phoneNumber = 'Please enter a valid phone number';
    }

    if (formData.bio && formData.bio.length > 1000) {
      newErrors.bio = 'Bio must be less than 1000 characters';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    try {
      const variables = {
        [isEditing ? 'updateCoachInput' : 'createCoachInput']: {
          ...(isEditing && { coachesLocDpxid: coach.coachesLocDpxid }),
          fullName: formData.fullName.trim(),
          email: formData.email.trim().toLowerCase(),
          phoneNumber: formData.phoneNumber.trim() || null,
          bio: formData.bio.trim() || null
        }
      };

      if (isEditing) {
        await updateCoach({ variables });
      } else {
        await createCoach({ variables });
      }
    } catch (error) {
      // Error handled in mutation callbacks
    }
  };

  return (
    <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              <i className={`bi ${isEditing ? 'bi-pencil' : 'bi-plus-circle'} me-2`}></i>
              {isEditing ? 'Edit Coach' : 'Create New Coach'}
            </h5>
            <button 
              type="button" 
              className="btn-close" 
              onClick={onClose}
              disabled={loading}
            ></button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              {errors.general && (
                <div className="alert alert-danger">
                  <i className="bi bi-exclamation-triangle-fill me-2"></i>
                  {errors.general}
                </div>
              )}

              <div className="row g-3">
                {/* Full Name */}
                <div className="col-md-6">
                  <label htmlFor="fullName" className="form-label">
                    Full Name <span className="text-danger">*</span>
                  </label>
                  <input
                    type="text"
                    id="fullName"
                    name="fullName"
                    className={`form-control ${errors.fullName ? 'is-invalid' : ''}`}
                    value={formData.fullName}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="Enter full name"
                    maxLength="100"
                  />
                  {errors.fullName && <div className="invalid-feedback">{errors.fullName}</div>}
                </div>

                {/* Email */}
                <div className="col-md-6">
                  <label htmlFor="email" className="form-label">
                    Email <span className="text-danger">*</span>
                  </label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                    value={formData.email}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="coach@example.com"
                    maxLength="100"
                  />
                  {errors.email && <div className="invalid-feedback">{errors.email}</div>}
                </div>

                {/* Phone Number */}
                <div className="col-md-6">
                  <label htmlFor="phoneNumber" className="form-label">
                    Phone Number
                  </label>
                  <input
                    type="tel"
                    id="phoneNumber"
                    name="phoneNumber"
                    className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`}
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="+1 (555) 123-4567"
                    maxLength="20"
                  />
                  {errors.phoneNumber && <div className="invalid-feedback">{errors.phoneNumber}</div>}
                </div>

                {/* Bio */}
                <div className="col-12">
                  <label htmlFor="bio" className="form-label">
                    Biography
                  </label>
                  <textarea
                    id="bio"
                    name="bio"
                    className={`form-control ${errors.bio ? 'is-invalid' : ''}`}
                    rows="4"
                    value={formData.bio}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="Tell us about this coach's background, specialties, and experience..."
                    maxLength="1000"
                  />
                  <div className="form-text">
                    {formData.bio.length}/1000 characters
                  </div>
                  {errors.bio && <div className="invalid-feedback">{errors.bio}</div>}
                </div>
              </div>

              {/* Preview Card */}
              {(formData.fullName || formData.email) && (
                <div className="mt-4">
                  <h6 className="text-muted">Preview:</h6>
                  <div className="card bg-light">
                    <div className="card-body p-3">
                      <div className="d-flex align-items-center">
                        <div className="avatar-circle me-3">
                          <i className="bi bi-person-fill"></i>
                        </div>
                        <div>
                          <h6 className="mb-1">{formData.fullName || 'Full Name'}</h6>
                          <p className="text-muted mb-0 small">{formData.email || 'email@example.com'}</p>
                          {formData.phoneNumber && (
                            <p className="text-muted mb-0 small">
                              <i className="bi bi-telephone me-1"></i>
                              {formData.phoneNumber}
                            </p>
                          )}
                        </div>
                      </div>
                      {formData.bio && (
                        <p className="mt-2 mb-0 small text-muted">
                          {formData.bio.substring(0, 100)}
                          {formData.bio.length > 100 && '...'}
                        </p>
                      )}
                    </div>
                  </div>
                </div>
              )}
            </div>

            <div className="modal-footer">
              <button 
                type="button" 
                className="btn btn-secondary" 
                onClick={onClose}
                disabled={loading}
              >
                Cancel
              </button>
              <button 
                type="submit" 
                className="btn btn-primary"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                    {isEditing ? 'Updating...' : 'Creating...'}
                  </>
                ) : (
                  <>
                    <i className={`bi ${isEditing ? 'bi-check-lg' : 'bi-plus-lg'} me-2`}></i>
                    {isEditing ? 'Update Coach' : 'Create Coach'}
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>

      <style jsx>{`
        .avatar-circle {
          width: 50px;
          height: 50px;
          border-radius: 50%;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 1.5rem;
          color: white;
        }
      `}</style>
    </div>
  );
};

export default CoachForm;