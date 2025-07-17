import React, { useState, useEffect } from 'react';
import { useQuery, useMutation } from '@apollo/client';
import { GET_CHATS, SEARCH_CHATS, DELETE_CHAT } from '../../services/graphqlQueries';
import { useAuth } from '../../context/AuthContext';
import Pagination from '../Common/Pagination';
import ChatForm from './ChatForm';

const ChatList = () => {
  const { isAdmin } = useAuth();
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchFilters, setSearchFilters] = useState({
    message: '',
    messageType: '',
    sentBy: ''
  });
  const [isSearching, setIsSearching] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingChat, setEditingChat] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [chatToDelete, setChatToDelete] = useState(null);

  // Query for regular chat loading
  const { data: chatsData, loading: chatsLoading, refetch: refetchChats } = useQuery(GET_CHATS, {
    variables: { currentPage, pageSize },
    skip: isSearching,
    notifyOnNetworkStatusChange: true
  });

  // Query for search
  const { data: searchData, loading: searchLoading, refetch: refetchSearch } = useQuery(SEARCH_CHATS, {
    variables: {
      request: {
        currentPage,
        pageSize,
        message: searchFilters.message || null,
        messageType: searchFilters.messageType || null,
        sentBy: searchFilters.sentBy || null
      }
    },
    skip: !isSearching,
    notifyOnNetworkStatusChange: true
  });

  // Delete mutation
  const [deleteChat] = useMutation(DELETE_CHAT, {
    onCompleted: () => {
      setShowDeleteModal(false);
      setChatToDelete(null);
      refetchData();
    },
    onError: (error) => {
      console.error('Delete error:', error);
      alert('Failed to delete chat: ' + (error.message || 'Unknown error'));
    }
  });

  const data = isSearching ? searchData : chatsData;
  const loading = isSearching ? searchLoading : chatsLoading;
  // const pagination = data?.searchChatsWithPaging || data?.getChatsWithPaging;
  const items = isSearching ? data?.searchChatsWithPaging?.items : data?.getChatsLocDpxes;
const pagination = isSearching ? data?.searchChatsWithPaging : {
  items: data?.getChatsLocDpxes || [],
  totalPages: 1,
  currentPage: 1,
  totalItems: data?.getChatsLocDpxes?.length || 0,
  pageSize: data?.getChatsLocDpxes?.length || 0
};

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
      message: '',
      messageType: '',
      sentBy: ''
    });
    setCurrentPage(1);
    setIsSearching(false);
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleEdit = (chat) => {
    setEditingChat(chat);
    setShowCreateModal(true);
  };

  const handleDelete = (chat) => {
    setChatToDelete(chat);
    setShowDeleteModal(true);
  };

  const confirmDelete = () => {
    if (chatToDelete) {
      deleteChat({ variables: { id: chatToDelete.chatsLocDpxid } });
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
              <p className="text-muted mb-0">Manage and monitor chat conversations</p>
            </div>
            <button 
              className="btn btn-primary"
              onClick={() => setShowCreateModal(true)}
            >
              <i className="bi bi-plus-circle me-2"></i>
              New Chat
            </button>
          </div>

          {/* Search Filters */}
          <div className="card mb-4">
            <div className="card-body">
              <div className="row g-3">
                <div className="col-md-4">
                  <label className="form-label">Message Content</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Search in messages..."
                    value={searchFilters.message}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, message: e.target.value }))}
                  />
                </div>
                <div className="col-md-3">
                  <label className="form-label">Message Type</label>
                  <select
                    className="form-select"
                    value={searchFilters.messageType}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, messageType: e.target.value }))}
                  >
                    <option value="">All Types</option>
                    <option value="text">Text</option>
                    <option value="image">Image</option>
                    <option value="file">File</option>
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
                <div className="col-md-2 d-flex align-items-end">
                  <div className="btn-group w-100">
                    <button 
                      className="btn btn-outline-primary"
                      onClick={handleSearch}
                      disabled={loading}
                    >
                      <i className="bi bi-search"></i>
                    </button>
                    <button 
                      className="btn btn-outline-secondary"
                      onClick={handleClearSearch}
                      disabled={loading}
                    >
                      <i className="bi bi-arrow-clockwise"></i>
                    </button>
                  </div>
                </div>
              </div>
              {isSearching && (
                <div className="mt-2">
                  <small className="text-info">
                    <i className="bi bi-funnel me-1"></i>
                    Search results active
                  </small>
                </div>
              )}
            </div>
          </div>

          {/* Chat Table */}
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
                          <th>Coach</th>
                          <th>User</th>
                          <th>Type</th>
                          <th>Sent By</th>
                          <th>Status</th>
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
                              <div style={{ maxWidth: '200px' }}>
                                <span className="text-truncate d-block" title={chat.message}>
                                  {chat.message}
                                </span>
                              </div>
                            </td>
                            <td>
                              <div>
                                <div className="fw-semibold">{chat.coach?.fullName}</div>
                                <small className="text-muted">{chat.coach?.email}</small>
                              </div>
                            </td>
                            <td>
                              <div>
                                <div className="fw-semibold">{chat.user?.fullName}</div>
                                <small className="text-muted">{chat.user?.email}</small>
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
                              <small className="text-muted">
                                {formatDate(chat.createdAt)}
                              </small>
                            </td>
                            <td>
                              <div className="btn-group btn-group-sm">
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
                <p>Are you sure you want to delete this chat message?</p>
                <div className="bg-light p-3 rounded">
                  <strong>Message:</strong> {chatToDelete?.message}
                </div>
                <p className="text-danger mt-3 mb-0">
                  <small>This action cannot be undone.</small>
                </p>
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={() => setShowDeleteModal(false)}
                >
                  Cancel
                </button>
                <button 
                  type="button" 
                  className="btn btn-danger" 
                  onClick={confirmDelete}
                >
                  <i className="bi bi-trash me-2"></i>
                  Delete
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