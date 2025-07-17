import React, { useState } from 'react';
import { useQuery, useMutation } from '@apollo/client';
import { GET_COACHES, SEARCH_COACHES, DELETE_COACH } from '../../services/graphqlQueries';
import { useAuth } from '../../context/AuthContext';
import Pagination from '../Common/Pagination';
import CoachForm from './CoachForm';

const CoachList = () => {
  const { isAdmin } = useAuth();
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchFilters, setSearchFilters] = useState({
    fullName: '',
    email: ''
  });
  const [isSearching, setIsSearching] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingCoach, setEditingCoach] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [coachToDelete, setCoachToDelete] = useState(null);
  const [selectedCoach, setSelectedCoach] = useState(null);

  // Query for regular coach loading
  const { data: coachesData, loading: coachesLoading, refetch: refetchCoaches } = useQuery(GET_COACHES, {
    variables: { currentPage, pageSize },
    skip: isSearching,
    notifyOnNetworkStatusChange: true
  });

  // Query for search
  const { data: searchData, loading: searchLoading, refetch: refetchSearch } = useQuery(SEARCH_COACHES, {
    variables: {
      fullName: searchFilters.fullName || null,
      email: searchFilters.email || null,
      currentPage,
      pageSize
    },
    skip: !isSearching,
    notifyOnNetworkStatusChange: true
  });

  // Delete mutation
  const [deleteCoach] = useMutation(DELETE_COACH, {
    onCompleted: () => {
      setShowDeleteModal(false);
      setCoachToDelete(null);
      refetchData();
    },
    onError: (error) => {
      console.error('Delete error:', error);
      alert('Failed to delete coach: ' + (error.message || 'Unknown error'));
    }
  });

  const data = isSearching ? searchData : coachesData;
  const loading = isSearching ? searchLoading : coachesLoading;
  const pagination = data?.searchCoachesWithPaging || data?.getCoachesWithPaging;

  const refetchData = () => {
    if (isSearching) {
      refetchSearch();
    } else {
      refetchCoaches();
    }
  };

  const handleSearch = () => {
    setCurrentPage(1);
    setIsSearching(true);
  };

  const handleClearSearch = () => {
    setSearchFilters({
      fullName: '',
      email: ''
    });
    setCurrentPage(1);
    setIsSearching(false);
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleView = (coach) => {
    setSelectedCoach(coach);
  };

  const handleEdit = (coach) => {
    setEditingCoach(coach);
    setShowCreateModal(true);
  };

  const handleDelete = (coach) => {
    setCoachToDelete(coach);
    setShowDeleteModal(true);
  };

  const confirmDelete = () => {
    if (coachToDelete) {
      deleteCoach({ variables: { id: coachToDelete.coachesLocDpxid } });
    }
  };

  const handleModalClose = () => {
    setShowCreateModal(false);
    setEditingCoach(null);
    refetchData();
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="container-fluid">
      <div className="row">
        <div className="col-12">
          {/* Header */}
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h2>
                <i className="bi bi-people text-primary me-2"></i>
                Coach Management
              </h2>
              <p className="text-muted mb-0">Manage coach profiles and information</p>
            </div>
            <button 
              className="btn btn-primary"
              onClick={() => setShowCreateModal(true)}
            >
              <i className="bi bi-plus-circle me-2"></i>
              New Coach
            </button>
          </div>

          {/* Search Filters */}
          <div className="card mb-4">
            <div className="card-body">
              <div className="row g-3">
                <div className="col-md-4">
                  <label className="form-label">Full Name</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Search by name..."
                    value={searchFilters.fullName}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, fullName: e.target.value }))}
                  />
                </div>
                <div className="col-md-4">
                  <label className="form-label">Email</label>
                  <input
                    type="email"
                    className="form-control"
                    placeholder="Search by email..."
                    value={searchFilters.email}
                    onChange={(e) => setSearchFilters(prev => ({ ...prev, email: e.target.value }))}
                  />
                </div>
                <div className="col-md-4 d-flex align-items-end">
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
                    Search results active
                  </small>
                </div>
              )}
            </div>
          </div>

          {/* Coach Cards/Table */}
          <div className="card">
            <div className="card-body">
              {loading ? (
                <div className="text-center py-5">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                  <p className="mt-3">Loading coaches...</p>
                </div>
              ) : pagination?.items?.length > 0 ? (
                <>
                  {/* Card View for Coaches */}
                  <div className="row g-4 mb-4">
                    {pagination.items.map((coach) => (
                      <div key={coach.coachesLocDpxid} className="col-lg-4 col-md-6">
                        <div className="card h-100 coach-card shadow-sm">
                          <div className="card-body">
                            <div className="d-flex align-items-center mb-3">
                              <div className="avatar-circle me-3">
                                <i className="bi bi-person-fill"></i>
                              </div>
                              <div className="flex-grow-1">
                                <h5 className="card-title mb-1">{coach.fullName}</h5>
                                <span className="badge bg-primary">
                                  <i className="bi bi-patch-check-fill me-1"></i>
                                  Certified Coach
                                </span>
                              </div>
                            </div>
                            
                            <div className="coach-info">
                              <div className="mb-2">
                                <i className="bi bi-envelope text-muted me-2"></i>
                                <span className="text-truncate">{coach.email}</span>
                              </div>
                              {coach.phoneNumber && (
                                <div className="mb-2">
                                  <i className="bi bi-telephone text-muted me-2"></i>
                                  <span>{coach.phoneNumber}</span>
                                </div>
                              )}
                              <div className="mb-3">
                                <i className="bi bi-calendar text-muted me-2"></i>
                                <small className="text-muted">
                                  Joined {formatDate(coach.createdAt)}
                                </small>
                              </div>
                              
                              {coach.bio && (
                                <div className="mb-3">
                                  <p className="text-muted small mb-0" style={{ maxHeight: '60px', overflow: 'hidden' }}>
                                    {coach.bio}
                                  </p>
                                </div>
                              )}
                            </div>
                          </div>
                          
                          <div className="card-footer bg-transparent">
                            <div className="d-flex justify-content-between align-items-center">
                              <small className="text-muted">
                                <i className="bi bi-hash"></i>{coach.coachesLocDpxid}
                              </small>
                              <div className="btn-group btn-group-sm">
                                <button
                                  className="btn btn-outline-info"
                                  onClick={() => handleView(coach)}
                                  title="View Details"
                                >
                                  <i className="bi bi-eye"></i>
                                </button>
                                <button
                                  className="btn btn-outline-primary"
                                  onClick={() => handleEdit(coach)}
                                  title="Edit"
                                >
                                  <i className="bi bi-pencil"></i>
                                </button>
                                {isAdmin() && (
                                  <button
                                    className="btn btn-outline-danger"
                                    onClick={() => handleDelete(coach)}
                                    title="Delete"
                                  >
                                    <i className="bi bi-trash"></i>
                                  </button>
                                )}
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
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
                  <i className="bi bi-people fs-1 text-muted"></i>
                  <p className="mt-3 text-muted">
                    {isSearching ? 'No coaches found matching your search criteria.' : 'No coaches available.'}
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

      {/* Coach Form Modal */}
      {showCreateModal && (
        <CoachForm
          coach={editingCoach}
          onClose={handleModalClose}
        />
      )}

      {/* Coach Details Modal */}
      {selectedCoach && (
        <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  <i className="bi bi-person-circle me-2"></i>
                  Coach Details
                </h5>
                <button 
                  type="button" 
                  className="btn-close" 
                  onClick={() => setSelectedCoach(null)}
                ></button>
              </div>
              <div className="modal-body">
                <div className="row">
                  <div className="col-md-4 text-center mb-3">
                    <div className="avatar-circle-large mx-auto mb-3">
                      <i className="bi bi-person-fill"></i>
                    </div>
                    <h4>{selectedCoach.fullName}</h4>
                    <span className="badge bg-primary">Certified Coach</span>
                  </div>
                  <div className="col-md-8">
                    <div className="mb-3">
                      <label className="fw-bold">Email:</label>
                      <p className="mb-1">{selectedCoach.email}</p>
                    </div>
                    {selectedCoach.phoneNumber && (
                      <div className="mb-3">
                        <label className="fw-bold">Phone:</label>
                        <p className="mb-1">{selectedCoach.phoneNumber}</p>
                      </div>
                    )}
                    {selectedCoach.bio && (
                      <div className="mb-3">
                        <label className="fw-bold">Biography:</label>
                        <p className="mb-1">{selectedCoach.bio}</p>
                      </div>
                    )}
                    <div className="mb-3">
                      <label className="fw-bold">Member Since:</label>
                      <p className="mb-1">{formatDate(selectedCoach.createdAt)}</p>
                    </div>
                    <div className="mb-3">
                      <label className="fw-bold">Coach ID:</label>
                      <p className="mb-1">#{selectedCoach.coachesLocDpxid}</p>
                    </div>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={() => setSelectedCoach(null)}
                >
                  Close
                </button>
                <button 
                  type="button" 
                  className="btn btn-primary" 
                  onClick={() => {
                    setSelectedCoach(null);
                    handleEdit(selectedCoach);
                  }}
                >
                  <i className="bi bi-pencil me-2"></i>
                  Edit Coach
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
                  Delete Coach
                </h5>
                <button 
                  type="button" 
                  className="btn-close" 
                  onClick={() => setShowDeleteModal(false)}
                ></button>
              </div>
              <div className="modal-body">
                <p>Are you sure you want to delete this coach?</p>
                <div className="bg-light p-3 rounded">
                  <strong>Name:</strong> {coachToDelete?.fullName}<br/>
                  <strong>Email:</strong> {coachToDelete?.email}
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

      <style jsx>{`
        .coach-card {
          transition: transform 0.2s ease-in-out;
        }
        
        .coach-card:hover {
          transform: translateY(-2px);
        }
        
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

        .avatar-circle-large {
          width: 100px;
          height: 100px;
          border-radius: 50%;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 3rem;
          color: white;
        }
      `}</style>
    </div>
  );
};

export default CoachList;