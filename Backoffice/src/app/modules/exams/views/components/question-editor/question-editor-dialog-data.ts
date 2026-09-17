import { Question, QuestionSupport } from "../../../models/question";

export interface QuestionEditorDialogData {
  question?: Question;

  availableQuestionSupports: QuestionSupport[];
}
