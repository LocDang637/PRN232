// src/services/graphqlQueries.js
import { gql } from '@apollo/client';

// Authentication
export const LOGIN_MUTATION = gql`
  mutation Login($username: String!, $password: String!) {
    login(username: $username, password: $password) {
      token
      user {
        userAccountId
        userName
        fullName
        email
        phone
        employeeCode
        roleId
        isActive
        createdDate
      }
    }
  }
`;

export const GET_CURRENT_USER = gql`
  query GetCurrentUser {
    getCurrentUser {
      userAccountId
      userName
      fullName
      email
      phone
      employeeCode
      roleId
      isActive
      createdDate
    }
  }
`;

// Chat Queries
export const GET_CHATS = gql`
  query GetChatsWithPaging($currentPage: Int!, $pageSize: Int!) {
    getChatsWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        chatsLocDpxid
        userId
        coachId
        message
        sentBy
        messageType
        isRead
        attachmentUrl
        responseTime
        createdAt
        coach {
          coachesLocDpxid
          fullName
          email
        }
        user {
          userAccountId
          userName
          fullName
          email
        }
      }
    }
  }
`;

export const SEARCH_CHATS = gql`
  query SearchChatsWithPaging($request: ClassSearchChatRequestInput!) {
    searchChatsWithPaging(request: $request) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        chatsLocDpxid
        userId
        coachId
        message
        sentBy
        messageType
        isRead
        attachmentUrl
        responseTime
        createdAt
        coach {
          coachesLocDpxid
          fullName
          email
        }
        user {
          userAccountId
          userName
          fullName
          email
        }
      }
    }
  }
`;

export const GET_CHAT_BY_ID = gql`
  query GetChatById($id: Int!) {
    chatsLocDpxById(id: $id) {
      chatsLocDpxid
      userId
      coachId
      message
      sentBy
      messageType
      isRead
      attachmentUrl
      responseTime
      createdAt
      coach {
        coachesLocDpxid
        fullName
        email
      }
      user {
        userAccountId
        userName
        fullName
        email
      }
    }
  }
`;

// Chat Mutations
export const CREATE_CHAT = gql`
  mutation CreateChat($input: ChatsLocDpxInput!) {
    createChatsLocDpx(createChatsLocDpxInput: $input)
  }
`;

export const UPDATE_CHAT = gql`
  mutation UpdateChat($input: ChatsLocDpxUpdateInput!) {
    updateChatsLocDpx(updateChatsLocDpxInput: $input)
  }
`;

export const DELETE_CHAT = gql`
  mutation DeleteChat($id: Int!) {
    deleteChatsLocDpx(id: $id)
  }
`;

// Coach Queries
export const GET_COACHES = gql`
  query GetCoachesWithPaging($currentPage: Int!, $pageSize: Int!) {
    getCoachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        coachesLocDpxid
        fullName
        email
        phoneNumber
        bio
        createdAt
      }
    }
  }
`;

export const SEARCH_COACHES = gql`
  query SearchCoachesWithPaging($fullName: String, $email: String, $currentPage: Int!, $pageSize: Int!) {
    searchCoachesWithPaging(fullName: $fullName, email: $email, currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        coachesLocDpxid
        fullName
        email
        phoneNumber
        bio
        createdAt
      }
    }
  }
`;

export const GET_COACH_BY_ID = gql`
  query GetCoachById($id: Int!) {
    coachesLocDpxById(id: $id) {
      coachesLocDpxid
      fullName
      email
      phoneNumber
      bio
      createdAt
    }
  }
`;

// Coach Mutations
export const CREATE_COACH = gql`
  mutation CreateCoach($input: CoachesLocDpxInput!) {
    createCoachesLocDpx(createCoachInput: $input)
  }
`;

export const UPDATE_COACH = gql`
  mutation UpdateCoach($input: CoachesLocDpxUpdateInput!) {
    updateCoachesLocDpx(updateCoachInput: $input)
  }
`;

export const DELETE_COACH = gql`
  mutation DeleteCoach($id: Int!) {
    deleteCoachesLocDpx(id: $id)
  }
`;

// Get all coaches for dropdown (simple list)
export const GET_ALL_COACHES_SIMPLE = gql`
  query GetAllCoaches {
    coachesLocDpxes {
      coachesLocDpxid
      fullName
      email
    }
  }
`;