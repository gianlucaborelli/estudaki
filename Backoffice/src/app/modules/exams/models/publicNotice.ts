import { Exam } from "./exam";
import { ExamCategory } from "./exam-category";

export interface PublicNotice {
  id: string;
  number: string | null;
  year: number;
  examinerOrganization: string | null;
  contractingOrganization: string | null;
  examCategory: ExamCategory | null;
  isReviewed: boolean;
  isPublished: boolean;
  fileUrl: string | null;
  exams: Exam[];
  questionCount: number | null;
  createdAt: string;
}
