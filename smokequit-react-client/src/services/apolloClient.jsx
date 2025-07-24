// src/services/apolloClient.js (Vite version)
import { ApolloClient, InMemoryCache, createHttpLink, from } from '@apollo/client';
import { setContext } from '@apollo/client/link/context';
import { onError } from '@apollo/client/link/error';

// HTTP link to your GraphQL endpoint
const httpLink = createHttpLink({
  uri: import.meta.env.VITE_GRAPHQL_URI || 'https://localhost:7045/graphql',
});

// Auth link to include JWT token in headers
const authLink = setContext((_, { headers }) => {
  const token = localStorage.getItem('authToken');
  
  return {
    headers: {
      ...headers,
      'Content-Type': 'application/json',
      authorization: token ? `Bearer ${token}` : "",
    }
  }
});

// Error link for handling GraphQL errors
const errorLink = onError(({ graphQLErrors, networkError, operation, forward }) => {
  if (graphQLErrors) {
    graphQLErrors.forEach(({ message, locations, path }) => {
      console.error(`GraphQL Error: ${message}`, { locations, path });
      
      // Handle specific GraphQL errors
      if (message.includes('Authorize')) {
        // Token might be expired or invalid
        localStorage.removeItem('authToken');
        localStorage.removeItem('currentUser');
        window.location.href = '/login';
      }
    });
  }

  if (networkError) {
    console.error('Network Error:', networkError);
    
    // Handle 401 Unauthorized
    if (networkError.statusCode === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('currentUser');
      window.location.href = '/login';
    }
    
    // Handle SSL/HTTPS issues in development
    if (networkError.message && networkError.message.includes('fetch')) {
      console.warn('Network fetch error - check if GraphQL server is running on https://localhost:7045/graphql');
    }
  }
});

// Create Apollo Client
const client = new ApolloClient({
  link: from([errorLink, authLink, httpLink]),
  cache: new InMemoryCache({
    typePolicies: {
      Query: {
        fields: {
          chatsWithPaging: {
            // Don't cache pagination results to ensure fresh data
            merge(existing, incoming) {
              return incoming;
            }
          },
          coachesWithPaging: {
            // Don't cache pagination results to ensure fresh data
            merge(existing, incoming) {
              return incoming;
            }
          },
          searchChatsWithPaging: {
            // Don't cache search results
            merge(existing, incoming) {
              return incoming;
            }
          },
          searchCoachesWithPaging: {
            // Don't cache search results
            merge(existing, incoming) {
              return incoming;
            }
          }
        }
      }
    }
  }),
  defaultOptions: {
    watchQuery: {
      errorPolicy: 'all',
      notifyOnNetworkStatusChange: true
    },
    query: {
      errorPolicy: 'all',
      notifyOnNetworkStatusChange: true
    },
    mutate: {
      errorPolicy: 'all'
    }
  }
});

export default client;