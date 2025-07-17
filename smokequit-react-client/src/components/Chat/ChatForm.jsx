import React, { useState, useEffect } from 'react';
import { useMutation, useQuery } from '@apollo/client';
import { CREATE_CHAT, UPDATE_CHAT, GET_ALL_COACHES_SIMPLE } from '../../services/graphqlQueries';
import { useAuth } from '../../context/AuthContext';

const ChatForm = ({ chat, onClose }) => {
  const { user } = useAuth();
  const isEditing = !!chat;

  const [formData, setFormData] = useState({
    userId: '',
    coachId: '',
    message: '',
    sentBy: 'User',
    messageType: 'text',
    isRead: false,
    attachmentUrl: '',
    responseTime: ''
  });

  const [errors, setErrors] = useState({});

  // Get coaches for dropdown
  const { data: coachesData, loading: coachesLoading } = useQuery(GET_ALL_COACHES_SIMPLE);

  const [createChat, { loading: createLoading }] = useMutation(CREATE_CHAT, {
    onCompleted: () => {
      onClose();
    },
    onError: (error) => {
      console.error('Create error:', error);
      setErrors({ general: error.message || 'Failed to create chat' });
    }
  });

  const [updateChat, { loading: updateLoading }] = useMutation(UPDATE_CHAT, {
    onCompleted: () => {
      onClose();
    },
    onError: (error) => {
      console.error('Update error:', error);
      setErrors({ general: error.message || 'Failed to update chat' });
    }
  });

  const loading = createLoading || updateLoading;

  useEffect(() => {
    if (isEditing && chat) {
      setFormData({
        userId: chat.userId || '',
        coachId: chat.coachId || '',
        message: chat.message || '',
        sentBy: chat.sentBy || 'User',
        messageType: chat.messageType || 'text',
        isRead: chat.isRead || false,
        attachmentUrl: chat.attachmentUrl || '',
        responseTime: chat.responseTime ? new Date(chat.responseTime).toISOString().slice(0, 16) : ''
      });
    } else {
      // Set default user ID for new chats
      setFormData(prev => ({
        ...prev,
        userId: user?.userAccountId || ''
      }));
    }
  }, [isEditing, chat, user]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
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

    if (!formData.userId) newErrors.userId = 'User is required';
    if (!formData.coachId) newErrors.coachId = 'Coach is required';
    if (!formData.message.trim()) newErrors.message = 'Message is required';
    if (!formData.sentBy) newErrors.sentBy = 'Sent By is required';
    if (!formData.messageType) newErrors.messageType = 'Message Type is required';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    try {
      const variables = {
        input: {
          userId: parseInt(formData.userId),
          coachId: parseInt(formData.coachId),
          message: formData.message.trim(),
          sentBy: formData.sentBy,
          messageType: formData.messageType,
          isRead: formData.isRead,
          attachmentUrl: formData.attachmentUrl.trim() || null,
          responseTime: formData.responseTime ? new Date(formData.responseTime).toISOString() : null
        }
      };

      if (isEditing) {
        variables.input.chatsLocDpxid = chat.chatsLocDpxid;
        await updateChat({ variables });
      } else {
        await createChat({ variables });
      }
    } catch (error) {
      // Error handled in mutation callbacks
    }
  };

  const coaches = coachesData?.coachesLocDpxes || [];

  return (
    <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              <i className={`bi ${isEditing ? 'bi-pencil' : 'bi-plus-circle'} me-2`}></i>
              {isEditing ? 'Edit Chat' : 'Create New Chat'}
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
                {/* Coach Selection */}
                <div className="col-md-6">
                  <label htmlFor="coachId" className="form-label">
                    Coach <span className="text-danger">*</span>
                  </label>
                  <select
                    id="coachId"
                    name="coachId"
                    className={`form-select ${errors.coachId ? 'is-invalid' : ''}`}
                    value={formData.coachId}
                    onChange={handleChange}
                    disabled={loading || coachesLoading}
                  >
                    <option value="">Select a coach...</option>
                    {coaches.map(coach => (
                      <option key={coach.coachesLocDpxid} value={coach.coachesLocDpxid}>
                        {coach.fullName} ({coach.email})
                      </option>
                    ))}
                  </select>
                  {errors.coachId && <div className="invalid-feedback">{errors.coachId}</div>}
                </div>

                {/* User ID (for admin/editing) */}
                <div className="col-md-6">
                  <label htmlFor="userId" className="form-label">
                    User ID <span className="text-danger">*</span>
                  </label>
                  <input
                    type="number"
                    id="userId"
                    name="userId"
                    className={`form-control ${errors.userId ? 'is-invalid' : ''}`}
                    value={formData.userId}
                    onChange={handleChange}
                    disabled={loading}
                    min="1"
                  />
                  {errors.userId && <div className="invalid-feedback">{errors.userId}</div>}
                </div>

                {/* Message */}
                <div className="col-12">
                  <label htmlFor="message" className="form-label">
                    Message <span className="text-danger">*</span>
                  </label>
                  <textarea
                    id="message"
                    name="message"
                    className={`form-control ${errors.message ? 'is-invalid' : ''}`}
                    rows="4"
                    value={formData.message}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="Enter the chat message..."
                    maxLength="1000"
                  />
                  <div className="form-text">
                    {formData.message.length}/1000 characters
                  </div>
                  {errors.message && <div className="invalid-feedback">{errors.message}</div>}
                </div>

                {/* Sent By */}
                <div className="col-md-4">
                  <label htmlFor="sentBy" className="form-label">
                    Sent By <span className="text-danger">*</span>
                  </label>
                  <select
                    id="sentBy"
                    name="sentBy"
                    className={`form-select ${errors.sentBy ? 'is-invalid' : ''}`}
                    value={formData.sentBy}
                    onChange={handleChange}
                    disabled={loading}
                  >
                    <option value="User">User</option>
                    <option value="Coach">Coach</option>
                  </select>
                  {errors.sentBy && <div className="invalid-feedback">{errors.sentBy}</div>}
                </div>

                {/* Message Type */}
                <div className="col-md-4">
                  <label htmlFor="messageType" className="form-label">
                    Message Type <span className="text-danger">*</span>
                  </label>
                  <select
                    id="messageType"
                    name="messageType"
                    className={`form-select ${errors.messageType ? 'is-invalid' : ''}`}
                    value={formData.messageType}
                    onChange={handleChange}
                    disabled={loading}
                  >
                    <option value="text">Text</option>
                    <option value="image">Image</option>
                    <option value="file">File</option>
                  </select>
                  {errors.messageType && <div className="invalid-feedback">{errors.messageType}</div>}
                </div>

                {/* Is Read */}
                <div className="col-md-4">
                  <div className="form-check mt-4">
                    <input
                      type="checkbox"
                      id="isRead"
                      name="isRead"
                      className="form-check-input"
                      checked={formData.isRead}
                      onChange={handleChange}
                      disabled={loading}
                    />
                    <label htmlFor="isRead" className="form-check-label">
                      Mark as Read
                    </label>
                  </div>
                </div>

                {/* Attachment URL */}
                <div className="col-md-6">
                  <label htmlFor="attachmentUrl" className="form-label">
                    Attachment URL
                  </label>
                  <input
                    type="url"
                    id="attachmentUrl"
                    name="attachmentUrl"
                    className="form-control"
                    value={formData.attachmentUrl}
                    onChange={handleChange}
                    disabled={loading}
                    placeholder="https://example.com/file.pdf"
                  />
                </div>

                {/* Response Time */}
                <div className="col-md-6">
                  <label htmlFor="responseTime" className="form-label">
                    Response Time
                  </label>
                  <input
                    type="datetime-local"
                    id="responseTime"
                    name="responseTime"
                    className="form-control"
                    value={formData.responseTime}
                    onChange={handleChange}
                    disabled={loading}
                  />
                </div>
              </div>
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
                    {isEditing ? 'Update Chat' : 'Create Chat'}
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ChatForm;