import { Exam } from "./exam";

export interface PublicNotice {
  id: string;
  number: string | null;
  year: number;
  examinerOrganization: string | null;
  contractingOrganization: string | null;
  examCategory: string | null;
  isReviewed: boolean;
  isPublished: boolean;
  fileUrl: string | null;
  exams: Exam[];
  questionCount: number | null;
  createdAt: string;
}
