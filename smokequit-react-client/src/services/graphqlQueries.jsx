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

// ===== BASIC CHAT QUERIES (Start with minimal fields) =====
export const GET_ALL_CHATS_BASIC = gql`
  query GetAllChatsBasic {
    chatsLocDpxes {
      chatsLocDpxid
      message
      sentBy
      messageType
      isRead
      createdAt
    }
  }
`;

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
    }
  }
`;

// Try with relations only if basic works
export const GET_ALL_CHATS_WITH_RELATIONS = gql`
  query GetAllChatsWithRelations {
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
      user {
        userAccountId
        userName
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
    }
  }
`;

// ===== BASIC COACH QUERIES =====
export const GET_ALL_COACHES_BASIC = gql`
  query GetAllCoachesBasic {
    coachesLocDpxes {
      coachesLocDpxid
      fullName
      email
    }
  }
`;

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

// ===== PAGINATION QUERIES (Test these second) =====
export const GET_CHATS_WITH_PAGING = gql`
  query GetChatsWithPaging($currentPage: Int, $pageSize: Int) {
    chatsWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        chatsLocDpxid
        message
        sentBy
        messageType
        isRead
        createdAt
      }
    }
  }
`;

export const GET_COACHES_WITH_PAGING = gql`
  query GetCoachesWithPaging($currentPage: Int, $pageSize: Int) {
    coachesWithPaging(currentPage: $currentPage, pageSize: $pageSize) {
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

// ===== SEARCH QUERIES =====
export const SEARCH_CHATS_WITH_PAGING = gql`
  query SearchChatsWithPaging($request: ClassSearchChatRequest!) {
    searchChatsWithPaging(request: $request) {
      totalPages
      currentPage
      pageSize
      totalItems
      items {
        chatsLocDpxid
        message
        sentBy
        messageType
        isRead
        createdAt
      }
    }
  }
`;

export const SEARCH_COACHES_WITH_PAGING = gql`
  query SearchCoachesWithPaging($fullName: String, $email: String, $currentPage: Int, $pageSize: Int) {
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

// ===== MUTATIONS =====
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

// ===== CONVENIENCE EXPORTS =====
// Since basic works, let's try with relations
export const GET_CHATS = GET_ALL_CHATS_WITH_RELATIONS;
export const GET_COACHES = GET_ALL_COACHES_BASIC;
export const GET_ALL_COACHES_SIMPLE = GET_ALL_COACHES_BASIC;
export const SEARCH_CHATS = SEARCH_CHATS_WITH_PAGING;