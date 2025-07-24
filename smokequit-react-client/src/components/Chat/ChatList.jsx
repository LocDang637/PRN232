import React, { useState } from 'react';
import { useQuery, useMutation } from '@apollo/client';
import { 
  GET_CHATS_WITH_PAGING, 
  SEARCH_CHATS_WITH_PAGING, 
  DELETE_CHAT 
} from '../../services/graphqlQueries';
import { useAuth } from '../../context/AuthContext';
import Pagination from '../Common/Pagination';
import ChatForm from './ChatForm';

const ChatList = () => {
  const { user, isAdmin } = useAuth();
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchFilters, setSearchFilters] = useState({
    messageType: '',
    sentBy: '',
    isRead: null
  });
  const [isSearching, setIsSearching] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingChat, setEditingChat] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [chatToDelete, setChatToDelete] = useState(null);
  const [selectedChat, setSelectedChat] = useState(null);
  const [errors, setErrors] = useState({});

  // Query for regular chat loading
  const { data: chatsData, loading: chatsLoading, refetch: refetchChats } = useQuery(GET_CHATS_WITH_PAGING, {
    variables: { currentPage, pageSize },
    skip: isSearching,
    notifyOnNetworkStatusChange: true,
    errorPolicy: 'all'
  });

  // Query for search
  const { data: searchData, loading: searchLoading, refetch: refetchSearch } = useQuery(SEARCH_CHATS_WITH_PAGING, {
    variables: {
      request: {
        messageType: searchFilters.messageType || null,
        sentBy: searchFilters.sentBy || null,
        isRead: searchFilters.isRead,
        currentPage,
        pageSize
      }
    },
    skip: !isSearching,
    notifyOnNetworkStatusChange: true,
    errorPolicy: 'all'
  });

  // Delete mutation
  const [deleteChat, { loading: deleteLoading }] = useMutation(DELETE_CHAT, {
    onCompleted: (data) => {
      console.log('Delete completed:', data);
      setShowDeleteModal(false);
      setChatToDelete(null);
      refetchData();
    },
    onError: (error) => {
      console.error('Delete error:', error);
      setErrors({ general: error.message || 'Failed to delete chat' });
      // Don't close modal on error so user can see the error
    }
  });

  const data = isSearching ? searchData : chatsData;
  const loading = isSearching ? searchLoading : chatsLoading;
  const pagination = data?.searchChatsWithPaging || data?.chatsWithPaging;

  const refetchData = () => {
    if (isSearching) {
      refetchSearch();
    } else {
      refetchChats();
    }
  };

  const handleSearch = () => {
    setCurrentPage(1);
    setIsSearching(true);
  };

  const handleClearSearch = () => {
    setSearchFilters({
      messageType: '',
      sentBy: '',
      isRead: null
    });
    setCurrentPage(1);
    setIsSearching(false);
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleView = (chat) => {
    setSelectedChat(chat);
  };

  const handleEdit = (chat) => {
    setEditingChat(chat);
    setShowCreateModal(true);
  };

  const handleDelete = (chat) => {
    setChatToDelete(chat);
    setShowDeleteModal(true);
  };

  const confirmDelete = async () => {
    if (chatToDelete) {
      console.log('Attempting to delete chat:', chatToDelete.chatsLocDpxid);
      setErrors({}); // Clear any previous errors
      try {
        await deleteChat({ 
          variables: { id: chatToDelete.chatsLocDpxid },
          errorPolicy: 'all' 
        });
      } catch (error) {
        console.error('Delete mutation error:', error);
        setErrors({ general: error.message || 'Failed to delete chat' });
      }
    }
  };

  const handleModalClose = () => {
    setShowCreateModal(false);
    setEditingChat(null);
    refetchData();
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  };

  const getMessageTypeVariant = (type) => {
    switch (type?.toLowerCase()) {
      case 'text': return 'primary';
      case 'image': return 'success';
      case 'file': return 'warning';
      default: return 'secondary';
    }
  };

  const getSentByVariant = (sentBy) => {
    return sentBy?.toLowerCase() === 'user' ? 'info' : 'dark';
  };

  const truncateMessage = (message, maxLength = 100) => {
    if (!message) return '';
    return message.length > maxLength ? message.substring(0, maxLength) + '...' : message;
  };

  return (
    <div className="container-fluid">
      <div className="row">
        <div className="col-12">
          {/* Header */}
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h2>
                <i className="bi bi-chat-dots text-primary me-2"></i>
                Chat Management
              </h2>
              <p className="text-muted mb-0">Manage chat conversations and messages</p>
            </div>
            <button 
              className="btn btn-primary"
              onClick={() => setShowCreateModal(true)}
            >
              <i className="bi bi-plus-circle me-2"></i>
              New Chat
            </button>
          </div>

          {/* User Info */}
          <div className="alert alert-info mb-4">
            <strong>Logged in as:</strong> {user?.fullName} ({user?.email}) - Role: {user?.roleId === 1 ? 'Admin' : 'User'}
          </div>

          {/* Search Filters */}
          <div className="card mb-4">
            <div className="card-body">
              <div className="row g-3">
                <div className="col-md-3">
                  <label className="form-label">Message Type</label>
                  <select
                    className="form-select"
                    value={searchFilters.messageType}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, messageType: e.target.value }))}
                  >
                    <option value="">All Types</option>
                    <option value="Text">Text</option>
                    <option value="Image">Image</option>
                    <option value="File">File</option>
                  </select>
                </div>
                <div className="col-md-3">
                  <label className="form-label">Sent By</label>
                  <select
                    className="form-select"
                    value={searchFilters.sentBy}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, sentBy: e.target.value }))}
                  >
                    <option value="">All</option>
                    <option value="User">User</option>
                    <option value="Coach">Coach</option>
                  </select>
                </div>
                <div className="col-md-3">
                  <label className="form-label">Read Status</label>
                  <select
                    className="form-select"
                    value={searchFilters.isRead === null ? '' : searchFilters.isRead.toString()}
                    onChange={(e) => setSearchFilters(prev => ({ 
                      ...prev, 
                      isRead: e.target.value === '' ? null : e.target.value === 'true' 
                    }))}
                  >
                    <option value="">All</option>
                    <option value="true">Read</option>
                    <option value="false">Unread</option>
                  </select>
                </div>
                <div className="col-md-3 d-flex align-items-end">
                  <div className="btn-group w-100">
                    <button 
                      className="btn btn-outline-primary"
                      onClick={handleSearch}
                      disabled={loading}
                    >
                      <i className="bi bi-search me-2"></i>
                      Search
                    </button>
                    <button 
                      className="btn btn-outline-secondary"
                      onClick={handleClearSearch}
                      disabled={loading}
                    >
                      <i className="bi bi-arrow-clockwise me-2"></i>
                      Clear
                    </button>
                  </div>
                </div>
              </div>
              {isSearching && (
                <div className="mt-2">
                  <small className="text-info">
                    <i className="bi bi-funnel me-1"></i>
                    Search results active - Filtering by: {
                      [
                        searchFilters.messageType && `Type: ${searchFilters.messageType}`,
                        searchFilters.sentBy && `Sent By: ${searchFilters.sentBy}`,
                        searchFilters.isRead !== null && `Status: ${searchFilters.isRead ? 'Read' : 'Unread'}`
                      ].filter(Boolean).join(', ') || 'All chats'
                    }
                  </small>
                </div>
              )}
            </div>
          </div>

          {/* Chat List */}
          <div className="card">
            <div className="card-body">
              {loading ? (
                <div className="text-center py-5">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                  <p className="mt-3">Loading chats...</p>
                </div>
              ) : pagination?.items?.length > 0 ? (
                <>
                  <div className="table-responsive">
                    <table className="table table-hover">
                      <thead className="table-light">
                        <tr>
                          <th>ID</th>
                          <th>Message</th>
                          <th>Type</th>
                          <th>Sent By</th>
                          <th>Status</th>
                          <th>Coach</th>
                          <th>Created</th>
                          <th>Actions</th>
                        </tr>
                      </thead>
                      <tbody>
                        {pagination.items.map((chat) => (
                          <tr key={chat.chatsLocDpxid}>
                            <td>
                              <span className="badge bg-light text-dark">
                                #{chat.chatsLocDpxid}
                              </span>
                            </td>
                            <td>
                              <div style={{ maxWidth: '300px' }}>
                                <span 
                                  className="text-truncate d-block" 
                                  title={chat.message}
                                  style={{ cursor: 'pointer' }}
                                  onClick={() => handleView(chat)}
                                >
                                  {truncateMessage(chat.message, 100)}
                                </span>
                              </div>
                            </td>
                            <td>
                              <span className={`badge bg-${getMessageTypeVariant(chat.messageType)}`}>
                                {chat.messageType}
                              </span>
                            </td>
                            <td>
                              <span className={`badge bg-${getSentByVariant(chat.sentBy)}`}>
                                {chat.sentBy}
                              </span>
                            </td>
                            <td>
                              <span className={`badge ${chat.isRead ? 'bg-success' : 'bg-warning'}`}>
                                {chat.isRead ? 'Read' : 'Unread'}
                              </span>
                            </td>
                            <td>
                              {chat.coach ? (
                                <div>
                                  <div className="fw-semibold">{chat.coach.fullName}</div>
                                  <small className="text-muted">{chat.coach.email}</small>
                                </div>
                              ) : (
                                <small className="text-muted">N/A</small>
                              )}
                            </td>
                            <td>
                              <small className="text-muted">
                                {formatDate(chat.createdAt)}
                              </small>
                            </td>
                            <td>
                              <div className="btn-group btn-group-sm">
                                <button
                                  className="btn btn-outline-info"
                                  onClick={() => handleView(chat)}
                                  title="View Details"
                                >
                                  <i className="bi bi-eye"></i>
                                </button>
                                <button
                                  className="btn btn-outline-primary"
                                  onClick={() => handleEdit(chat)}
                                  title="Edit"
                                >
                                  <i className="bi bi-pencil"></i>
                                </button>
                                {isAdmin() && (
                                  <button
                                    className="btn btn-outline-danger"
                                    onClick={() => handleDelete(chat)}
                                    title="Delete"
                                  >
                                    <i className="bi bi-trash"></i>
                                  </button>
                                )}
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>

                  {/* Pagination */}
                  <Pagination
                    currentPage={pagination.currentPage}
                    totalPages={pagination.totalPages}
                    totalItems={pagination.totalItems}
                    pageSize={pagination.pageSize}
                    onPageChange={handlePageChange}
                    loading={loading}
                  />
                </>
              ) : (
                <div className="text-center py-5">
                  <i className="bi bi-chat-dots fs-1 text-muted"></i>
                  <p className="mt-3 text-muted">
                    {isSearching ? 'No chats found matching your search criteria.' : 'No chats available.'}
                  </p>
                  {isSearching && (
                    <button className="btn btn-outline-primary" onClick={handleClearSearch}>
                      Clear Search
                    </button>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Chat Form Modal */}
      {showCreateModal && (
        <ChatForm
          chat={editingChat}
          onClose={handleModalClose}
        />
      )}

      {/* Chat Details Modal */}
      {selectedChat && (
        <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  <i className="bi bi-chat-square-text me-2"></i>
                  Chat Details
                </h5>
                <button 
                  type="button" 
                  className="btn-close" 
                  onClick={() => setSelectedChat(null)}
                ></button>
              </div>
              <div className="modal-body">
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="fw-bold">Chat ID:</label>
                      <p className="mb-1">#{selectedChat.chatsLocDpxid}</p>
                    </div>
                    <div className="mb-3">
                      <label className="fw-bold">Message Type:</label>
                      <p className="mb-1">
                        <span className={`badge bg-${getMessageTypeVariant(selectedChat.messageType)}`}>
                          {selectedChat.messageType}
                        </span>
                      </p>
                    </div>
                    <div className="mb-3">
                      <label className="fw-bold">Sent By:</label>
                      <p className="mb-1">
                        <span className={`badge bg-${getSentByVariant(selectedChat.sentBy)}`}>
                          {selectedChat.sentBy}
                        </span>
                      </p>
                    </div>
                    <div className="mb-3">
                      <label className="fw-bold">Status:</label>
                      <p className="mb-1">
                        <span className={`badge ${selectedChat.isRead ? 'bg-success' : 'bg-warning'}`}>
                          {selectedChat.isRead ? 'Read' : 'Unread'}
                        </span>
                      </p>
                    </div>
                  </div>
                  <div className="col-md-6">
                    {selectedChat.coach && (
                      <div className="mb-3">
                        <label className="fw-bold">Coach:</label>
                        <p className="mb-1">{selectedChat.coach.fullName}</p>
                        <small className="text-muted">{selectedChat.coach.email}</small>
                      </div>
                    )}
                    <div className="mb-3">
                      <label className="fw-bold">Created:</label>
                      <p className="mb-1">{formatDate(selectedChat.createdAt)}</p>
                    </div>
                    {selectedChat.responseTime && (
                      <div className="mb-3">
                        <label className="fw-bold">Response Time:</label>
                        <p className="mb-1">{formatDate(selectedChat.responseTime)}</p>
                      </div>
                    )}
                  </div>
                </div>
                <div className="mb-3">
                  <label className="fw-bold">Message:</label>
                  <div className="bg-light p-3 rounded">
                    {selectedChat.message}
                  </div>
                </div>
                {selectedChat.attachmentUrl && (
                  <div className="mb-3">
                    <label className="fw-bold">Attachment:</label>
                    <p className="mb-1">
                      <a href={selectedChat.attachmentUrl} target="_blank" rel="noopener noreferrer">
                        {selectedChat.attachmentUrl}
                      </a>
                    </p>
                  </div>
                )}
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={() => setSelectedChat(null)}
                >
                  Close
                </button>
                <button 
                  type="button" 
                  className="btn btn-primary" 
                  onClick={() => {
                    setSelectedChat(null);
                    handleEdit(selectedChat);
                  }}
                >
                  <i className="bi bi-pencil me-2"></i>
                  Edit Chat
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteModal && (
        <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  <i className="bi bi-exclamation-triangle text-danger me-2"></i>
                  Delete Chat
                </h5>
                <button 
                  type="button" 
                  className="btn-close" 
                  onClick={() => setShowDeleteModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                {errors.general && (
                  <div className="alert alert-danger">
                    <i className="bi bi-exclamation-triangle-fill me-2"></i>
                    {errors.general}
                  </div>
                )}
                <p>Are you sure you want to delete this chat?</p>
                <div className="bg-light p-3 rounded">
                  <strong>ID:</strong> #{chatToDelete?.chatsLocDpxid}<br/>
                  <strong>Message:</strong> {truncateMessage(chatToDelete?.message, 100)}
                </div>
                <p className="text-danger mt-3 mb-0">
                  <small>This action cannot be undone.</small>
                </p>
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={() => {
                    setShowDeleteModal(false);
                    setErrors({});
                  }}
                  disabled={deleteLoading}
                >
                  Cancel
                </button>
                <button 
                  type="button" 
                  className="btn btn-danger" 
                  onClick={confirmDelete}
                  disabled={deleteLoading}
                >
                  {deleteLoading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                      Deleting...
                    </>
                  ) : (
                    <>
                      <i className="bi bi-trash me-2"></i>
                      Delete
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatList;