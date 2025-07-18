import React, { useState } from 'react';
import { useQuery } from '@apollo/client';
import { 
  GET_ALL_CHATS_BASIC, 
  GET_ALL_CHATS, 
  GET_ALL_CHATS_WITH_RELATIONS,
  GET_CHATS_WITH_PAGING 
} from '../../services/graphqlQueries';
import { useAuth } from '../../context/AuthContext';

const ChatList = () => {
  const { user } = useAuth();
  const [selectedQuery, setSelectedQuery] = useState('basic');

  // Define all test queries
  const queries = {
    basic: {
      name: 'Basic Chats (Minimal Fields)',
      query: GET_ALL_CHATS_BASIC,
      variables: {}
    },
    all: {
      name: 'All Chats (No Relations)',
      query: GET_ALL_CHATS,
      variables: {}
    },
    relations: {
      name: 'Chats with Relations',
      query: GET_ALL_CHATS_WITH_RELATIONS,
      variables: {}
    },
    paging: {
      name: 'Chats with Pagination',
      query: GET_CHATS_WITH_PAGING,
      variables: { currentPage: 1, pageSize: 10 }
    }
  };

  const currentQuery = queries[selectedQuery];

  // Execute the selected query
  const { data, loading, error, refetch } = useQuery(currentQuery.query, {
    variables: currentQuery.variables,
    notifyOnNetworkStatusChange: true,
    errorPolicy: 'all'
  });

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

  // Extract items based on query type
  let items = [];
  if (selectedQuery === 'paging' && data?.chatsWithPaging?.items) {
    items = data.chatsWithPaging.items;
  } else if (data?.chatsLocDpxes) {
    items = data.chatsLocDpxes;
  }

  return (
    <div className="container-fluid">
      <div className="row">
        <div className="col-12">
          {/* Header */}
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h2>
                <i className="bi bi-bug text-warning me-2"></i>
                GraphQL Debug Tool
              </h2>
              <p className="text-muted mb-0">Testing different GraphQL queries step by step</p>
            </div>
            <div className="d-flex gap-2">
              <button 
                className="btn btn-outline-secondary"
                onClick={() => refetch()}
                disabled={loading}
              >
                <i className="bi bi-arrow-clockwise me-2"></i>
                Refresh
              </button>
            </div>
          </div>

          {/* User Info */}
          <div className="alert alert-info mb-4">
            <strong>Logged in as:</strong> {user?.fullName} ({user?.email}) - Role: {user?.roleId}
          </div>

          {/* Query Selector */}
          <div className="card mb-4">
            <div className="card-header">
              <h5 className="mb-0">Select Query to Test</h5>
            </div>
            <div className="card-body">
              <div className="row g-3">
                {Object.entries(queries).map(([key, query]) => (
                  <div key={key} className="col-md-3">
                    <button
                      className={`btn w-100 ${selectedQuery === key ? 'btn-primary' : 'btn-outline-primary'}`}
                      onClick={() => setSelectedQuery(key)}
                      disabled={loading}
                    >
                      {query.name}
                    </button>
                  </div>
                ))}
              </div>
              <div className="mt-3">
                <small className="text-muted">
                  <strong>Current:</strong> {currentQuery.name}
                </small>
              </div>
            </div>
          </div>

          {/* Query Details */}
          <div className="card mb-4">
            <div className="card-header">
              <h6 className="mb-0">Query Details</h6>
            </div>
            <div className="card-body">
              <div className="row">
                <div className="col-md-6">
                  <strong>Status:</strong>
                  <span className={`badge ms-2 ${loading ? 'bg-warning' : error ? 'bg-danger' : 'bg-success'}`}>
                    {loading ? 'Loading...' : error ? 'Error' : 'Success'}
                  </span>
                </div>
                <div className="col-md-6">
                  <strong>Items Found:</strong> {items.length}
                </div>
              </div>
              
              {/* Variables */}
              {Object.keys(currentQuery.variables).length > 0 && (
                <div className="mt-3">
                  <strong>Variables:</strong>
                  <pre className="bg-light p-2 rounded mt-1">
                    {JSON.stringify(currentQuery.variables, null, 2)}
                  </pre>
                </div>
              )}

              {/* Error Details */}
              {error && (
                <div className="mt-3">
                  <strong>Error Details:</strong>
                  <div className="alert alert-danger mt-2">
                    <strong>Message:</strong> {error.message}<br/>
                    {error.graphQLErrors?.map((err, index) => (
                      <div key={index} className="mt-2">
                        <strong>GraphQL Error {index + 1}:</strong> {err.message}<br/>
                        {err.path && <><strong>Path:</strong> {err.path.join(' → ')}<br/></>}
                        {err.locations && <><strong>Location:</strong> Line {err.locations[0]?.line}, Column {err.locations[0]?.column}</>}
                      </div>
                    ))}
                    {error.networkError && (
                      <div className="mt-2">
                        <strong>Network Error:</strong> {error.networkError.message}
                      </div>
                    )}
                  </div>
                </div>
              )}

              {/* Raw Data (for debugging) */}
              {data && (
                <div className="mt-3">
                  <button 
                    className="btn btn-sm btn-outline-info"
                    onClick={() => console.log('Full GraphQL Response:', data)}
                  >
                    Log Full Response to Console
                  </button>
                </div>
              )}
            </div>
          </div>

          {/* Results Table */}
          <div className="card">
            <div className="card-header">
              <h6 className="mb-0">Query Results</h6>
            </div>
            <div className="card-body">
              {loading ? (
                <div className="text-center py-5">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                  <p className="mt-3">Executing query...</p>
                </div>
              ) : error ? (
                <div className="text-center py-5">
                  <i className="bi bi-exclamation-triangle fs-1 text-danger"></i>
                  <p className="mt-3 text-danger">Query failed</p>
                  <p className="text-muted">Check the error details above</p>
                </div>
              ) : items?.length > 0 ? (
                <div className="table-responsive">
                  <table className="table table-hover">
                    <thead className="table-light">
                      <tr>
                        <th>ID</th>
                        <th>Message</th>
                        <th>Type</th>
                        <th>Sent By</th>
                        <th>Status</th>
                        <th>Created</th>
                        <th>Coach</th>
                        <th>User</th>
                      </tr>
                    </thead>
                    <tbody>
                      {items.map((chat) => (
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
                            {chat.user ? (
                              <div>
                                <div className="fw-semibold">{chat.user.fullName}</div>
                                <small className="text-muted">{chat.user.email}</small>
                              </div>
                            ) : (
                              <small className="text-muted">N/A</small>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <div className="text-center py-5">
                  <i className="bi bi-chat-dots fs-1 text-muted"></i>
                  <p className="mt-3 text-muted">No chats found</p>
                </div>
              )}
            </div>
          </div>

          {/* Instructions */}
          <div className="alert alert-primary mt-4">
            <h6>Testing Instructions:</h6>
            <ol>
              <li><strong>Start with "Basic Chats"</strong> - This has minimal fields and should work</li>
              <li><strong>Try "All Chats"</strong> - Adds more fields but no relations</li>
              <li><strong>Test "Chats with Relations"</strong> - Includes coach/user data</li>
              <li><strong>Finally "Chats with Pagination"</strong> - Tests the pagination query</li>
            </ol>
            <p className="mb-0">
              <strong>Goal:</strong> Find which query works, then we'll use that as the base for the real component.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatList;