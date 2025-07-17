// src/services/apolloClient.js (Vite version)
import { ApolloClient, InMemoryCache, createHttpLink, from } from '@apollo/client';
import { setContext } from '@apollo/client/link/context';
import { onError } from '@apollo/client/link/error';

// HTTP link to your GraphQL endpoint
const httpLink = createHttpLink({
  uri: import.meta.env.VITE_GRAPHQL_URI || 'https://localhost:7045/graphql/',
});

// Auth link to include JWT token in headers
const authLink = setContext((_, { headers }) => {
  const token = localStorage.getItem('authToken');
  
  return {
    headers: {
      ...headers,
      authorization: token ? `Bearer ${token}` : "",
    }
  }
});

// Error link for handling GraphQL errors
const errorLink = onError(({ graphQLErrors, networkError, operation, forward }) => {
  if (graphQLErrors) {
    graphQLErrors.forEach(({ message, locations, path }) => {
      console.error(`GraphQL Error: ${message}`, { locations, path });
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
  }
});

// Create Apollo Client
const client = new ApolloClient({
  link: from([errorLink, authLink, httpLink]),
  cache: new InMemoryCache(),
  defaultOptions: {
    watchQuery: {
      errorPolicy: 'all'
    },
    query: {
      errorPolicy: 'all'
    }
  }
});

export default client;