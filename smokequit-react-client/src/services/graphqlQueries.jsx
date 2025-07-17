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

// Chat Queries - Use existing method from your C# code
export const GET_CHATS = gql`
  query GetChatsWithPaging($currentPage: Int!, $pageSize: Int!) {
    getChatsLocDpxes {
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

// FIXED: Matches your C# ClassSearchChatRequest (PascalCase)
export const SEARCH_CHATS = gql`
  query SearchChatsWithPaging($request: ClassSearchChatRequest!) {
    SearchChatsWithPaging(request: $request) {
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
    GetChatsLocDpxById(id: $id) {
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

// Chat Mutations - FIXED parameter names to match C# schema (PascalCase)
export const CREATE_CHAT = gql`
  mutation CreateChat($input: ChatsLocDpxInput!) {
    CreateChatsLocDpx(createChatsLocDpxInput: $input)
  }
`;

export const UPDATE_CHAT = gql`
  mutation UpdateChat($input: ChatsLocDpxUpdateInput!) {
    UpdateChatsLocDpx(updateChatsLocDpxInput: $input)
  }
`;

export const DELETE_CHAT = gql`
  mutation DeleteChat($id: Int!) {
    DeleteChatsLocDpx(id: $id)
  }
`;

// Coach Queries - FIXED to match C# schema (PascalCase)
export const GET_COACHES = gql`
  query GetCoachesWithPaging($currentPage: Int!, $pageSize: Int!) {
    GetCoachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
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
    SearchCoachesWithPaging(fullName: $fullName, email: $email, currentPage: $currentPage, pageSize: $pageSize) {
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
    GetCoachesLocDpxById(id: $id) {
      coachesLocDpxid
      fullName
      email
      phoneNumber
      bio
      createdAt
    }
  }
`;

// Coach Mutations - FIXED parameter names to match C# schema (PascalCase)
export const CREATE_COACH = gql`
  mutation CreateCoach($input: CoachesLocDpxInput!) {
    CreateCoachesLocDpx(createCoachInput: $input)
  }
`;

export const UPDATE_COACH = gql`
  mutation UpdateCoach($input: CoachesLocDpxUpdateInput!) {
    UpdateCoachesLocDpx(updateCoachInput: $input)
  }
`;

export const DELETE_COACH = gql`
  mutation DeleteCoach($id: Int!) {
    DeleteCoachesLocDpx(id: $id)
  }
`;

// Get all coaches for dropdown - FIXED query name to match C# schema (PascalCase)
export const GET_ALL_COACHES_SIMPLE = gql`
  query GetAllCoaches {
    GetCoachesLocDpxes {
      coachesLocDpxid
      fullName
      email
    }
  }
`;