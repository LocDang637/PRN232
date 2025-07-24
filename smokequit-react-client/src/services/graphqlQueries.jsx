import { gql } from '@apollo/client';

// ===== AUTHENTICATION QUERIES & MUTATIONS =====
export const LOGIN_MUTATION = gql`
  mutation Login($username: String!, $password: String!) {
    login(username: $username, password: $password) {
      token
      user {
        userAccountId
        userName
        fullName
        email
        employeeCode
        roleId
        isActive
      }
    }
  }
`;

export const GET_CURRENT_USER = gql`
  query GetCurrentUser {
    currentUser {
      userAccountId
      userName
      fullName
      email
      employeeCode
      roleId
      isActive
    }
  }
`;

export const VALIDATE_TOKEN = gql`
  query ValidateToken($token: String!) {
    validateToken(token: $token)
  }
`;

// ===== CHAT QUERIES =====
export const GET_ALL_CHATS = gql`
  query GetAllChats {
    chatsLocDpxes {
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
    }
  }
`;

export const GET_CHATS_WITH_PAGING = gql`
  query GetChatsWithPaging($currentPage: Int!, $pageSize: Int!) {
    chatsWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      hasNextPage
      hasPreviousPage
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
      }
    }
  }
`;

export const SEARCH_CHATS_BY_MESSAGE = gql`
  query SearchChatsByMessage($messageContent: String!) {
    searchChatsByMessage(messageContent: $messageContent) {
      chatsLocDpxid
      userId
      coachId
      message
      sentBy
      messageType
      isRead
      createdAt
      coach {
        coachesLocDpxid
        fullName
        email
      }
    }
  }
`;

export const SEARCH_CHATS = gql`
  query SearchChats($messageContent: String, $messageType: String, $sentBy: String, $isRead: Boolean) {
    searchChats(messageContent: $messageContent, messageType: $messageType, sentBy: $sentBy, isRead: $isRead) {
      chatsLocDpxid
      userId
      coachId
      message
      sentBy
      messageType
      isRead
      createdAt
      coach {
        coachesLocDpxid
        fullName
        email
      }
    }
  }
`;

export const SEARCH_CHATS_WITH_PAGING = gql`
  query SearchChatsWithPaging($request: ClassSearchChatRequestInput!) {
    searchChatsWithPaging(request: $request) {
      totalPages
      currentPage
      pageSize
      totalItems
      hasNextPage
      hasPreviousPage
      items {
        chatsLocDpxid
        userId
        coachId
        message
        sentBy
        messageType
        isRead
        createdAt
        coach {
          coachesLocDpxid
          fullName
          email
        }
      }
    }
  }
`;

// ===== COACH QUERIES =====
export const GET_ALL_COACHES = gql`
  query GetAllCoaches {
    coachesLocDpxes {
      coachesLocDpxid
      fullName
      email
      phoneNumber
      bio
      createdAt
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

export const GET_COACH_BY_EMAIL = gql`
  query GetCoachByEmail($email: String!) {
    coachByEmail(email: $email) {
      coachesLocDpxid
      fullName
      email
      phoneNumber
      bio
      createdAt
    }
  }
`;

export const GET_COACHES_WITH_PAGING = gql`
  query GetCoachesWithPaging($currentPage: Int!, $pageSize: Int!) {
    coachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      hasNextPage
      hasPreviousPage
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
  query SearchCoaches($fullName: String, $email: String) {
    searchCoaches(fullName: $fullName, email: $email) {
      coachesLocDpxid
      fullName
      email
      phoneNumber
      bio
      createdAt
    }
  }
`;

export const SEARCH_COACHES_WITH_PAGING = gql`
  query SearchCoachesWithPaging($fullName: String, $email: String, $currentPage: Int!, $pageSize: Int!) {
    searchCoachesWithPaging(fullName: $fullName, email: $email, currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      hasNextPage
      hasPreviousPage
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

// ===== CHAT MUTATIONS =====
export const CREATE_CHAT = gql`
  mutation CreateChat($createChatsLocDpxInput: ChatsLocDpxInput!) {
    createChatsLocDpx(createChatsLocDpxInput: $createChatsLocDpxInput)
  }
`;

export const UPDATE_CHAT = gql`
  mutation UpdateChat($updateChatsLocDpxInput: ChatsLocDpxUpdateInput!) {
    updateChatsLocDpx(updateChatsLocDpxInput: $updateChatsLocDpxInput)
  }
`;

export const DELETE_CHAT = gql`
  mutation DeleteChat($id: Int!) {
    deleteChatsLocDpx(id: $id)
  }
`;

// ===== COACH MUTATIONS =====
export const CREATE_COACH = gql`
  mutation CreateCoach($createCoachInput: CoachesLocDpxInput!) {
    createCoachesLocDpx(createCoachInput: $createCoachInput)
  }
`;

export const UPDATE_COACH = gql`
  mutation UpdateCoach($updateCoachInput: CoachesLocDpxUpdateInput!) {
    updateCoachesLocDpx(updateCoachInput: $updateCoachInput)
  }
`;

export const DELETE_COACH = gql`
  mutation DeleteCoach($id: Int!) {
    deleteCoachesLocDpx(id: $id)
  }
`;

// ===== USER ACCOUNT QUERIES =====
export const GET_USER_ACCOUNT = gql`
  query GetUserAccount($username: String!, $password: String!) {
    userAccount(username: $username, password: $password) {
      userAccountId
      userName
      fullName
      email
      employeeCode
      roleId
      isActive
    }
  }
`;

export const GET_ALL_USERS = gql`
  query GetAllUsers {
    systemUserAccounts {
      userAccountId
      userName
      fullName
      email
      employeeCode
      roleId
      isActive
      createdDate
    }
  }
`;

// ===== CONVENIENCE EXPORTS =====
export const GET_CHATS = GET_CHATS_WITH_PAGING;
export const GET_COACHES = GET_COACHES_WITH_PAGING;
export const GET_ALL_COACHES_SIMPLE = GET_ALL_COACHES;