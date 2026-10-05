export interface Contact {
    name: string;
    email: string;
    message: string;
    canBeReplied: boolean;
    userId?: string;
}

export const CONTACT_MESSAGE_MAX_LENGTH = 500;

export interface ContactFormValues {
    name: string;
    email: string;
    message: string;
    canBeReplied: boolean;
}

export interface ContactFormErrors {
    name?: string;
    email?: string;
    message?: string;
    general?: string;
}
