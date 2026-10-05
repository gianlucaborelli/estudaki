export interface LoginModel {
  email: string;
  password: string;
}

export interface UserMeModel {
  isAuthenticated: boolean;
  userId?: string;
  [key: string]: unknown;
}
